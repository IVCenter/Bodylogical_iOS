/*
* UDPSocket.cs
* 
* Author: Danilo Gasques   (2018)
* Contact: gasques@ucsd.edu
*
* Based on: https://gist.github.com/danilogr/967bbaddbc6262f533c1d523fd888aa1
* 
* This is a two-way (receive&send) udp socket that works both on Hololens (UWP) and Unity editor (Windows).
* On UWP (Universal Windows Platform), it implements non-blocking IO through Async calls.
* On Unity / Windows Desktop, it uses a treaded implementation of a blocking UDP socket
* 
* This class was designed to handle text messages (and not binary messages)
*
*/

// NEFTX_CORE is "Defined when building scripts against .NET Core class libraries on .NET."
//(see more here: https://docs.unity3d.com/Manual/PlatformDependentCompilation.html)

using System;
using System.Collections.Generic;
using System.Text;

#if NETFX_CORE
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
#else
using System.Net.Sockets;
using System.Threading;
using System.Net;
#endif

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity event that supports a string as parameter
/// </summary>
[System.Serializable]
public class StringEvent : UnityEvent<String>
{
}



/// <summary>
/// UDPSocket wraps an unreliable, message-based socket (max ~2k)
/// </summary>
public class UDPSocket : MonoBehaviour
{

    // Static members

    /// <summary>
    /// Tag for logging
    /// </summary>
    static private string TagPrefix = "UDPSocket";
    static private int InstanceCount = 0;

    // Object members
    [Tooltip("Socket's ID (Each socket receives a unique id.)")]
    public readonly string TAG; // this socket's tag

    [Tooltip("Port number the socket should be using for listening")]
    public int listenPort = 50000;

    /// <summary>
    /// IP address of PC for UDP
    /// </summary>
    [Tooltip("IP that this socket will be sending messages")]
    public string externalIP = "192.168.0.10";

    /// <summary>
    /// Port number of PC for UDP
    /// </summary>
    [Tooltip("Port that this socket will be sending messages")]
    public int externalPort = 50005;



    /// <summary>
    /// Messages received are enqueued
    /// </summary>
    [HideInInspector]
    public Queue<String> messageQueue = new Queue<String>();

    [Tooltip("Event handler for messages received")]
    public StringEvent OnMessageReceived;

    public event Action<string> MessageReceived;


#if NETFX_CORE
    private DatagramSocket _impl;

    string externalPortStr = "50005";
#else
    private UdpClient _impl;
    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);  // listens to any address
    bool stopThread = false;
    Thread socketThread;
#endif

    UDPSocket()
    {
        // creates a unique id for this socket
        TAG = TagPrefix + (++InstanceCount);
    }

    // ================================================================================ Unity Events =========================================================== //
    /// <summary>
    /// Socket constructor, creates all socket objects and defines each socket unique id
    /// </summary>

#if NETFX_CORE
    public async Task Awake()
    {
        externalPortStr = externalPort.ToString();

        _impl = new DatagramSocket();
        _impl.MessageReceived += Socket_MessageReceived;

        // Set quality of service to low latency (we are using UDP after all)
        _impl.Control.QualityOfService = SocketQualityOfService.LowLatency;

        HostName IP = null;
        try {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            IP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
            .SingleOrDefault(
                hn =>
                    hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                    == icp.NetworkAdapter.NetworkAdapterId);

            await _impl.BindEndpointAsync(IP, listenPort.ToString());
        }
        catch (Exception e) {
            Debug.Log(e.ToString());
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            return;
        }

#else
    public void Awake()
    {
        _impl = new UdpClient(listenPort);
        socketThread = new Thread(new ThreadStart(SocketThreadLoop));
        socketThread.IsBackground = true;
        stopThread = false;
        socketThread.Start();
#endif

    }

    /// <summary>
    /// Called every frame
    /// </summary>
    void Update()
    {
#if !NETFX_CORE
        // only locks if not running on UWP due to possible conflicts with an external thread
        lock (messageQueue)
#endif
        {
            while (messageQueue.Count > 0)
            {
                // process message received
                String message = messageQueue.Dequeue();

                // calls the currently registered callback
                if (OnMessageReceived != null)
                {
                    OnMessageReceived.Invoke(message);
                    MessageReceived.Invoke(message);
                }

            }

        }

    }


    /// <summary>
    /// Called when the script is destroyed (see https://docs.unity3d.com/Manual/ExecutionOrder.html)
    /// </summary>
    private void OnDestroy()
    {
#if NETFX_CORE
            //await Task.Run(_impl.CancelIOAsync());
           _impl.Dispose();
#else
        stopThread = true;

        _impl.Close();
        if (socketThread != null)
            socketThread.Abort();  // aborts thread

#endif

    }

    // ========================================================================= Class Members =========================================================== //

    /// <summary>
    /// Sends message to destination port
    /// </summary>
    /// <param name="message">string to send</param>
#if NETFX_CORE
    public async Task  Send(string message)
    {
        try
        {
            using (var stream = await _impl.GetOutputStreamAsync(new Windows.Networking.HostName(externalIP), externalPortStr))
            {
                using (var writer = new Windows.Storage.Streams.DataWriter(stream))
                {
                    var data = Encoding.UTF8.GetBytes(message);

                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(TAG + " SendMessageUDP exception:\n" + e.ToString());
        }
#else
    public void Send(string message)
    {
        byte[] dgram = Encoding.UTF8.GetBytes(message);
        _impl.Send(dgram, dgram.Length, externalIP, externalPort);
#endif

    }

    /// <summary>
    /// Use this method to test whether you are receiving any messages or not
    /// </summary>
    public void TestHandler(String message)
    {
        Debug.Log(TAG + "" + message);
    }


    // ==================================== Code that only runs on the Unity Editor / Windows Desktop ======================================================//

#if !NETFX_CORE

    // Windows thread loop
    /// <summary>
    /// Socket Thread Loop for the socket version running on Windows
    /// </summary>
    private void SocketThreadLoop()
    {
        while (!stopThread)
        {
            try
            {
                String msg = System.Text.Encoding.UTF8.GetString(_impl.Receive(ref anyIP));
                lock (messageQueue)
                {
                    messageQueue.Enqueue(msg);
                }
            }
            catch (Exception err)
            {
                Debug.Log(TAG + err.ToString());
            }
        }
    }


#endif

    // ===================================================== Code that only runs on UWP  =====================================================================//

#if NETFX_CORE
    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
    Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        StreamReader reader = new StreamReader(streamIn);
        string message = await reader.ReadToEndAsync(); // this is a UDP message, so we can safely read all the lines
        messageQueue.Enqueue(message);
    }

#endif

}
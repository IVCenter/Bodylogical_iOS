using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    #region public members
    public string HostIp;
    public int HostPort;
    public event Action<string> MessageReceived;
    [HideInInspector]
    public Queue<String> messageQueue = new Queue<string>();
    public bool SetupSent { get; private set; }
    #endregion

    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion


    // Use this for initialization 	
    void Start()
    {
        ConnectToTcpServer();
        SetupSent = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!SetupSent)
        {
            if (socketConnection != null)
            {
                Debug.Log("Sending Setup Message");
                Send("{\"type\":\"setup\",\"holoCode\":\"yes, yes, I am holo\"}");
                SetupSent = true;
            }
        }

#if !NETFX_CORE
        // Lock if *not* on UWP to avoid thread conflicts
        lock (messageQueue)
#endif
        {
            while(messageQueue.Count > 0)
            {
                //process message received
                String message = messageQueue.Dequeue();

                //Trigger callback
                MessageReceived.Invoke(message);
            }
        }
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(HostIp, HostPort);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                        
                        lock (messageQueue)
                        {
                            messageQueue.Enqueue(serverMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    public void Send(string msg)
    {
        if (socketConnection == null)
        {
            Debug.Log("No socket connection");
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Send uint32 of message length to server
                UInt32 len = (UInt32) msg.Length;
                String len_str = len.ToString();
                while (len_str.Length < 4)
                {
                    len_str = "0" + len_str;
                }

                // Send message length
                //byte[] lenBytes = Encoding.UTF8.GetBytes(len_str);
                //stream.Write(lenBytes, 0, lenBytes.Length);

                // Send message
                byte[] msgBytes = Encoding.UTF8.GetBytes(len_str+msg);
                stream.Write(msgBytes, 0, msgBytes.Length);
                //Debug.Log("Sent TCP Message");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}

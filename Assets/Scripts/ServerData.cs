using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

using ZXing;
using ZXing.QrCode;
using UnityEngine.Windows;


public class ServerData : MonoBehaviour {
    
    [HideInInspector]
    public string serverIP = "100.81.39.171";
    [HideInInspector]
    public int serverPort = 3000;
    //public int UdpPort = 12345;
    [Tooltip("http://123.45.67.890:3000/holo")]
    public string urlOverrideInUnity = "";
    [HideInInspector]
    public string HoloLensIP = "100.81.34.222";

    private bool settingUp = true;
    private string https = "http://";
    WebCamTexture cam;

    private Vector3 lastUserPos = Vector3.zero;
    private float lastUserRot = 0;

    List<GameObject> poiObjects = new List<GameObject>();
    List<int> poiIds = new List<int>();
    int idIndex = 0;

    private void Start()
    {
#if NETFX_CORE
        cam = new WebCamTexture();
        cam.Play();
#else
        // If in Unity, check for an override url.
        if (urlOverrideInUnity.Length > 1)
        {
            // Skip looking for QR code and go straight to override url
            settingUp = false;
            Debug.Log("Using SketchManager.ServerData.urlOverrideInUnity url and bypassing QR Code\nDelete the url and replay to search for QR Code.");
            SetAddresses(urlOverrideInUnity);
            StartCoroutine(TouchUrl(urlOverrideInUnity));
        }
        else
        {
            cam = new WebCamTexture();
            cam.Play();
        }
#endif
    }

    private void Update()
    {
        if (settingUp)
        {
            string url = GetFrame();
            if (url != null)
            {
                SetAddresses(url);
                StartCoroutine(TouchUrl(url));
            }
        }
        else
        {
            // Send user location if head has moved significantly
            TransformJson headLoc = new TransformJson(Camera.main.transform);
            if (Vector3.Magnitude(headLoc.position - lastUserPos) > 0.2f ||
                System.Math.Abs(headLoc.rotation.eulerAngles.y - lastUserRot) > 1)
            {
                //Debug.Log("Sending Head Update");
                TCPClient tcp = GetComponent<TCPClient>();
                string msg = string.Format("{{\"type\":\"userLocation\",\"location\":{0}}}", headLoc.toSimpleJson());
                tcp.Send(msg);

                lastUserPos = Camera.main.transform.position;
                lastUserRot = Camera.main.transform.rotation.eulerAngles.y;
            }
        }
    }

    private void SetAddresses(string url)
    {
        // Get IP and Port for Server
        if (url.IndexOf("prototipar.io") != -1)
        {
            // get port
            url = url.Replace(https, "");
            serverIP = url.Split("//".ToCharArray())[0];
            serverPort = int.Parse(url.Split('#')[1]);
            HoloLensIP = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            Debug.Log(string.Format("Server at '{0}' port '{1}'", serverIP, serverPort));
        }
        else
        {
            url = url.Replace(https, "");
            serverIP = url.Split(':')[0];
            serverPort = int.Parse(url.Split(':')[1].Split("/\\".ToCharArray())[0]);
            HoloLensIP = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            //Debug.Log(string.Format("Server at '{0}' port '{1}'", serverIP, serverPort));
        }


        // Activate UDPSocket and ImageGenerator
        Debug.Log("HoloLens IP Address: " + Dns.GetHostAddresses(Dns.GetHostName())[0].ToString());
        //GetComponent<UDPSocket>().externalIP = serverIP;
        //GetComponent<UDPSocket>().externalPort = serverPort+1;
        //GetComponent<UDPSocket>().listenPort = UdpPort;
        //GetComponent<UDPSocket>().enabled = true;
        

        // Activate TCP Client
        GetComponent<TCPClient>().HostIp = serverIP;
        GetComponent<TCPClient>().HostPort = serverPort + 2;
        GetComponent<TCPClient>().enabled = true;

        GetComponent<ImageGenerator>().enabled = true;

        GameObject.FindGameObjectWithTag("CursorIndicatorText").GetComponent<UnityEngine.UI.Text>().text = "connected";
    }

    private IEnumerator TouchUrl(string url)
    {
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError) { print("Got Network Error"); }
        if (www.isHttpError) { print("Got HTTP Error"); }
        if (www.error != null)
        {
            Debug.Log("Failed to touch address: " + url + " error: "+www.error);
        }
        else
        {
            // Check got good data
            string data = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            if (data.StartsWith("HoloLens IP Set"))
            {
                Debug.Log("Touched " + url);
            }
            else
            {
                Debug.Log("Touched wrong url... "+url);
            }
        }
    }

    private string GetFrame()
    {
        Color32[] pix = cam.GetPixels32();
        return ReadQR(pix, cam.width, cam.height);
    }

    private string ReadQR(Texture2D tex)
    {
        BarcodeReader barcodeReader = new BarcodeReader();
        var result = barcodeReader.Decode(tex.GetPixels32(), tex.width, tex.height);
        if (result != null)
        {
            Debug.Log("Decoded Text from QR: " + result.Text);
            settingUp = false;
            return result.Text;
        }
        return null;
    }
    private string ReadQR(Color32[] pic, int width, int height)
    {
        BarcodeReader barcodeReader = new BarcodeReader();
        var result = barcodeReader.Decode(pic, width, height);
        if (result != null)
        {
            Debug.Log("Decoded Text from QR: " + result.Text);
            settingUp = false;
            cam.Stop();
            
            return result.Text;
        }
        return null;
    }


    public void SendTransform(GameObject sketch, string filename)
    {
        idIndex += 1;
        TCPClient tcp = GetComponent<TCPClient>();
        TransformUpdateJson trans = new TransformUpdateJson(sketch, filename, idIndex);
        poiObjects.Add(sketch);
        poiIds.Add(idIndex);
        tcp.Send(trans.toJson());
    }
}

[System.Serializable]
public class TransformJson
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public string toJson()
    {
        string json = "";

        json += "{\"position\":{\"x\":" + 
            position.x.ToString() + ",\"y\":" + 
            position.y.ToString() + ",\"z\":" + 
            position.z.ToString() + "},";
        json += "\"rotation\":{\"w\":" + 
            rotation.w.ToString() + ",\"x\":" + 
            rotation.x.ToString() + ",\"y\":" + 
            rotation.y.ToString() + ",\"z\":" + 
            rotation.z.ToString() + "},";
        json += "\"scale\":{\"x\":" + 
            scale.x.ToString() + ",\"y\":" + 
            scale.y.ToString() + ",\"z\":" + 
            scale.z.ToString() + "}}";
        return json;
    }

    public string toSimpleJson()
    {
        string json = "";

        json += "{\"position\":{\"x\":" +
            position.x.ToString() + ",\"y\":" +
            position.y.ToString() + ",\"z\":" +
            position.z.ToString() + "},";
        json += "\"rotation\":{\"w\":" +
            rotation.w.ToString() + ",\"x\":" +
            rotation.x.ToString() + ",\"y\":" +
            rotation.y.ToString() + ",\"z\":" +
            rotation.z.ToString() + ",\"flat\":" +
            rotation.eulerAngles.y.ToString() + "},";
        json += "\"scale\":{\"x\":" +
            scale.x.ToString() + ",\"y\":" +
            scale.y.ToString() + ",\"z\":" +
            scale.z.ToString() + "}}";
        return json;
    }

    public TransformJson(Transform transform)
    {
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
    }
}

[System.Serializable]
public class TransformUpdateJson
{
    public string type;
    public string filename;
    public int id;
    public TransformJson transform;

    public TransformUpdateJson(GameObject obj, string filename, int id)
    {
        type = "transformUpdate";
        this.filename = filename;
        this.id = id;
        transform = new TransformJson(obj.GetComponent<Transform>());
    }

    public string toJson()
    {
        string json = "";

        json += "{\"type\":\"" + type + "\",";
        json += "\"filename\":\"" + filename + "\",";
        json += "\"id\":\"" + id + "\",";
        json += "\"transform\":" + transform.toJson() + "}";

        return json;
    }
}

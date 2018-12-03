using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageDownloader : MonoBehaviour
{
    public int instanceIndex;
    public int[] framesActive;

    private string sketchUrl = "http://192.168.1.41:3000/sketch/1";
    [HideInInspector]
    public string baseUrl = "http://192.168.1.41:3000/";
    private int id = 1;
    public int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
            sketchUrl = baseUrl + "load/" + id;
        }
    }
    public void SetUrl(string url)
    {
        sketchUrl = url;
    }


    private Texture texture;

    void Start()
    {
        print(string.Format("Created Image DownloaderV2 tied to sketch {0}", id));

        sketchUrl = baseUrl + "load/" + id.ToString();

        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StartedManipulation += SketchDragStarted;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StoppedManipulation += SketchDropped;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StoppedManipulation += UpdateVarsOnSketchDragged;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StoppedManipulation += SendTransform;
    }

    private void UpdateVarsOnSketchDragged()
    {
        
    }

    private void SketchDropped()
    {
        // Tell Server the Transform of this object
    }

    private void SketchDragStarted()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(UpdateTexture(true));
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(UpdateTexture(false));
        }
    }

    #region Helper Methods

    /// <summary>
    /// Method to get an image from online server
    /// uses class sketchUrl variable as image location
    /// downloads image and sets it as the texture on this game object
    /// Also can place the image infront of the main camera
    /// </summary>
    /// <param name="updatePosition">Whether or not to place image infront of Player,
    /// Or to leave it in place and only update the texture</param>
    /// <returns></returns>
    public IEnumerator UpdateTexture(bool updatePosition)
    {
        UnityWebRequest www = UnityWebRequest.Get(sketchUrl);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            Debug.LogError(".");
            yield return null;
        }

        if (www.isNetworkError) { print("Got Network Error"); }
        if (www.isHttpError) { print("Got HTTP Error"); }
        if (www.error != null)
        {
            Debug.Log(string.Format("www.error: '{0}'\nat {1}", www.error, sketchUrl));
            if (www.error == "Generic/unknown HTTP error")
            {
                Debug.Log("Made an invalid request for a sketch. Used: " + sketchUrl);
                yield return null;
            }
        }
        else
        {
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true;

            // Get size
            String[] dims = new String[2];
            dims[0] = (String) www.GetResponseHeader("Content-Dims-X");
            dims[1] = (String) www.GetResponseHeader("Content-Dims-Y");

            // online image to texture
            byte[] textureData = www.downloadHandler.data;
            Texture2D tex = new Texture2D(1, 1); //texture size will be overridden when image loads
            tex.LoadImage(textureData);

            // calculate scale
            float targetSize = 20.0f; //cm
            float scaleX = targetSize / tex.width;
            float scaleY = targetSize / tex.height;
            this.transform.localScale = new Vector3(scaleY, 1, scaleX); // set scale, don't know why flip of x and y is needed, but it works

            tex.EncodeToPNG();
            texture = tex;

            GetComponent<Renderer>().material.mainTexture = texture;
            Debug.Log("Instance " + instanceIndex + " of Sketch " +id+ " Loaded");

            if (updatePosition) { placeSketch(); }
        }
    }

    


    /// <summary>
    /// Method to place the SketchPlane object in front of the player
    /// This is so when a plane is downloaded by a player
    /// they immediately see it in front of them
    /// </summary>
    void placeSketch()
    {
        float dist = 0.5f;
        Vector3 newPos = Camera.main.transform.position + Camera.main.transform.forward.normalized * dist;

        transform.position = newPos;

        // Rotate towards camera
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, 0, 0));
    }

    public void SendTransform()
    {
        GetComponentInParent<ServerData>().SendTransform(gameObject, "misc");
    }

    #endregion Helper Methods
}


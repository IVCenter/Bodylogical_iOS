using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using HoloToolkit.Examples.InteractiveElements;
using UnityEngine.XR.WSA.Input;

public class ImageGenerator : MonoBehaviour {

    private string ip;
    private int port;
    private string https = "http://";
    [HideInInspector]
    public int currentFrame;

    [Header("For IP addresses and Ports,\nsee the ServerData Component")]
    [Tooltip("Prefab SketchPlane with ImageDownloader script attached")]
    public GameObject sketchPlane;
    [Tooltip("Prefab number")]
    public GameObject TranslationPointPrefab;
    [Tooltip("InputManager which has the VideoCaputerHandler script attached")]
    public VideoCaptureHandler video;
    

    public event System.Action<int> pushSketch;

    private string baseUrl;
    private int numSketchPlanes;
    public List<GameObject> sketches = new List<GameObject>();

    // For Swipe
    private bool swiping;
    private Vector3 avgVel = new Vector3();
    private Vector3 lastPos = new Vector3();
    private uint sampleCounter = 0;
    private bool hadHandLastFrame;
    private Vector3 handLocation;
    private uint HandLostForNFrames;
    private float swipeStart;
    private float swipeTimeLength = 0.2f;
    private Vector3 initialVelocity;

    private int count;





    /// <summary>
    /// Initialization - setup url base to download images from and get the SketchPlane prefab
    /// url is based on an IP Address and a Port number which are controlled by Unity Editor.
    /// </summary>
    void Start () {
        ip = GetComponent<ServerData>().serverIP;
        port = GetComponent<ServerData>().serverPort;

        if (ip.IndexOf("prototipar.io") != -1) {
            baseUrl = https + ip + "/";
        }
        else
        {
            baseUrl = https + ip + ":" + port + "/";
        }
        Debug.Log(string.Format("Using {0} as base url.", baseUrl));
        numSketchPlanes = 0;

        //TODO: go to "http://" + ip + ":" + port + "/clients" and get number of pads
        //GeneratePlane for each one without changing location

        //pushSketch += getSketchPushedFromServer;
        //InteractionManager.InteractionSourceUpdated += handUpdated;

        GetComponent<TCPClient>().MessageReceived += HandlePackageDataFromNode;
    }

    

    /// <summary>
    /// checks every frame for 'N' key press. Calls generatePlane if key is pressed
    /// </summary>
    void Update () {
        
        // Manual Override for generating a plane
        if (Input.GetKeyDown(KeyCode.N)) { GeneratePlane(); }

        // Manual Override for generating particular interactions
        if (Input.GetKeyDown(KeyCode.Alpha1)) { GameObject sketch = GeneratePlane(); ProximityDisappear(sketch, 1.5f); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { GameObject sketch = GeneratePlane(); ProximityAppear(sketch, 1.5f); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { GameObject sketch = GeneratePlane(); GazeDisappear(sketch, 0.25f); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { GameObject sketch = GeneratePlane(); GazeAppear(sketch, 0.25f); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { GameObject sketch = GeneratePlane(); BillboardingAll(sketch); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { GameObject sketch = GeneratePlane(); LoopingSound(sketch, "Sounds/Granulizer"); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { GameObject sketch = GeneratePlane(); ProximitySound(sketch, "Sounds/GlassBreaking", 1.5f); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { GameObject sketch = GeneratePlane(); ProximitySilence(sketch, "Sounds/BabyCrying", 1.5f); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { GameObject sketch = GeneratePlane(); ProximityTranslate(sketch, 1.5f, 0.2f); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {; }

        // Manual Override for Swipe Push from server
        if (Input.GetKeyUp(KeyCode.Return)) {
            if (swiping) { swiping = false; Debug.Log("Swipe Ended"); }
            else { pushSketch.Invoke(1); }// ++count); }
        }

        //// ADD MORE WAYS OF TRIGGERING INTERACTIONS

        
    }

    #region Json Handling
    public void HandlePackageDataFromNode(string data)
    {
        Debug.Log("GOT MESSAGE: " + data);
        int typeStart = data.IndexOf("type") + "type':".Length;
        typeStart = data.IndexOf('"', typeStart)+1;
        int typeEnd = data.IndexOf('"', typeStart);
        string type = data.Substring(typeStart, typeEnd - typeStart);
        Debug.Log(string.Format("Got TCP Message of {0} type", type));

        switch (type)
        {
            case "manualTrigger":
                TriggerJson jsonTrigger = JsonUtility.FromJson<TriggerJson>(data);
                Debug.Log(string.Format("Manually Triggering {0} interactions for instance {1} of sketch {2}", jsonTrigger.trigger, jsonTrigger.sketchId, jsonTrigger.instanceIndex));
                for (int i=0; i<sketches.Count; i++)
                {
                    GameObject s = sketches[i];
                    if (s && s.GetComponent<ImageDownloader>().Id == jsonTrigger.sketchId &&
                        s.GetComponent<ImageDownloader>().instanceIndex == jsonTrigger.instanceIndex)
                    {
                        TriggerHandler[] handlers = s.GetComponentsInChildren<TriggerHandler>();
                        for (int h=0; h<handlers.Length; h++)
                        {
                            switch(jsonTrigger.trigger)
                            {
                                case "disappear_proximity":
                                    if (handlers[h].tag.Equals("SKETCH_DisappearOnCollision")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                                case "disappear_gaze":
                                    if (handlers[h].tag.Equals("SKETCH_DisappearWhenLookedAt")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                                case "appear_proximity":
                                    if (handlers[h].tag.Equals("SKETCH_AppearOnCollision")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                                case "appear_gaze":
                                    if (handlers[h].tag.Equals("SKETCH_AppearWhenLookedAt")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                                case "sound_approach":
                                    if (handlers[h].tag.Equals("SKETCH_SoundOnCollision")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                                case "sound_leave":
                                    if (handlers[h].tag.Equals("SKETCH_SoundOffOnCollision")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                                case "translate":
                                    if (handlers[h].tag.Equals("SKETCH_ProximityTranslation")) handlers[h].ManualTrigger(jsonTrigger.trigger);
                                    break;
                            }
                            
                        }
                    }
                }
                break;
            case "delete":
                DeleteJson jsonDel = JsonUtility.FromJson<DeleteJson>(data);
                Debug.Log(string.Format("Deleting instace {0} of sketch {1}", jsonDel.instanceIndex, jsonDel.sketchId));

                switch (jsonDel.instanceIndex)
                {
                    case "sketch":
                        goto case "instances";
                    case "instances":
                        for (int i = sketches.Count - 1; i >= 0; i--)
                        {
                            if (sketches[i].GetComponent<ImageDownloader>().Id == jsonDel.sketchId)
                            {
                                DeletePlane(sketches[i], false);
                            }
                        }
                        break;
                    default:
                        int inst = int.Parse(jsonDel.instanceIndex);
                        Debug.Log(string.Format("Attempting to Delete Instance {0} of Sketch {1}", inst, jsonDel.sketchId));

                        for (int i = sketches.Count - 1; i >= 0; i--)
                        {
                            int sketchId = sketches[i].GetComponent<ImageDownloader>().Id;
                            int sketchInst = sketches[i].GetComponent<ImageDownloader>().instanceIndex;
                            if (sketchId == jsonDel.sketchId && sketchInst == inst)
                            {
                                DeletePlane(sketches[i], false);
                                Debug.Log(string.Format("Deleted Sketch at Index {0} of ID {1} and Instance {2}\nTold to Delete Sketch {3} instance {4}", i, sketchId, sketchInst, jsonDel.sketchId, inst));
                                //break; //shouldn't need
                            }
                        }
                        break;
                }
                
                break;
            default:
                Debug.Log("Got unknown TCP Message Type: " + type);
                break;
        }
    }

    public void getInteractionsFromJson(InteractionsJson json, GameObject sketch, TranslationData updateTransform = null)
    {
        if (json.disappear)
        {
            if (json.disappear_proximity)
                ProximityDisappear(sketch, json.disappear_proximity_radius);
            if (json.disappear_gaze)
                GazeDisappear(sketch, json.disappear_gaze_radius);
        }
        if (json.appear)
        {
            if (json.appear_proximity)
                ProximityAppear(sketch, json.appear_proximity_radius);
            if (json.appear_gaze)
                GazeAppear(sketch, json.appear_gaze_radius);
        }
        if (json.sound)
        {
            if (json.sound_loop)
                LoopingSound(sketch, json.sound_loop_file);
            if (json.sound_approach)
                ProximitySound(sketch, json.sound_approach_file, json.sound_approach_radius);
            if (json.sound_leave)
                ProximitySilence(sketch, json.sound_leave_file, json.sound_leave_radius);
        }
        if (json.billboard)
            BillboardingAll(sketch);
        if (json.translate)
        {
            ProximityTranslate(sketch, json.translate_radius, json.translate_speed);
            if (updateTransform != null)
            {
                sketch.GetComponent<TranslationInteractionManager>().ImportData(updateTransform);
            }
        }

        Debug.Log("Interactions Set");
    }

    #endregion






    /// <summary>
    /// Methods for Sketch and other Object interactions
    /// Method names based on [player type][trigger type][behvior]
    /// player type will be left out unless the method is specific to a player
    /// </summary>
    #region Interactions



    /// <summary>
    /// Creates a SketchPlane GameObject by calling ColliderObjectSetup()
    /// Then creates a child with a Collider and Tag 'SKETCH_DisappearOnCollision'
    /// CameraManagerForSketches (On the main camera) will then look for that tag on collision
    /// </summary>
    public void ProximityDisappear(GameObject sketch, float radius)
    {
        GameObject collisionObject = ColliderObjectSetup(sketch, "SKETCH_DisappearOnCollision");
        collisionObject.AddComponent<SphereCollider>().isTrigger = true;
        collisionObject.GetComponent<SphereCollider>().radius = radius;
    }

    /// <summary>
    /// Creates a SketchPlane GameObject by calling ColliderObjectSetup()
    /// Then creates a child with a Collider and Tag 'SKETCH_AppearOnCollision'
    /// CameraManagerForSketches (On the main camera) will then look for that tag on collision
    /// </summary>
    public void ProximityAppear(GameObject sketch, float radius)
    {
        GameObject collisionObject = ColliderObjectSetup(sketch, "SKETCH_AppearOnCollision");
        collisionObject.GetComponentInParent<MeshRenderer>().enabled = false;
        collisionObject.GetComponentInParent<MeshCollider>().enabled = false;
        collisionObject.AddComponent<SphereCollider>().isTrigger = true;
        collisionObject.GetComponent<SphereCollider>().radius = radius;
    }

    /// <summary>
    /// Indended Usage: Sketch disappears when gaze touches it
    /// Creates a SketchPlane GameObject by calling ColliderObjectSetup()
    /// Then creates a child with a Collider and Tag 'SKETCH_DisappearWhenLookedAt'
    /// CameraManagerForSketches (On the main camera) will then look for that tag on collision
    /// </summary>
    public void GazeDisappear(GameObject sketch, float radius)
    {
        GameObject collisionObject = ColliderObjectSetup(sketch, "SKETCH_DisappearWhenLookedAt");
        collisionObject.layer = 0;
        collisionObject.AddComponent<SphereCollider>().isTrigger = true;
        collisionObject.GetComponent<SphereCollider>().radius = radius;

        //// Could also have when gaze touches the sketchplane itself
        /*
        MeshCollider sketchCollider = collisionObject.GetComponentInParent<MeshCollider>();
        collisionObject.AddComponent<MeshCollider>().isTrigger = true;
        collisionObject.GetComponent<MeshCollider>().sharedMesh = sketchCollider.sharedMesh;
        */
    }

    /// <summary>
    /// Intended Usage: Sketch appears when gaze touches it
    /// Creates a SketchPlane GameObject by calling ColliderObjectSetup()
    /// Then creates a child with a Collider and Tag 'SKETCH_AppearWhenLookedAt'
    /// CameraManagerForSketches (On the main camera) will then look for that tag on collision
    /// </summary>
    public void GazeAppear(GameObject sketch, float radius)
    {
        GameObject collisionObject = ColliderObjectSetup(sketch, "SKETCH_AppearWhenLookedAt");
        collisionObject.layer = 0;
        collisionObject.AddComponent<SphereCollider>().isTrigger = true;
        collisionObject.GetComponent<SphereCollider>().radius = radius;
        collisionObject.GetComponentInParent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Sketch plane is constantly facing the each user (aka Billboarding)
    /// in their own view
    /// Creates a SketchPlane GameObject by calling GeneratePlane()
    /// Adds a Billboarding component to the SketchPlane which updates its orientation
    /// to always face the user
    /// </summary>
    public void BillboardingAll(GameObject sketch)
    {
        sketch.tag = "SKETCH_BillboardAll";
        sketch.AddComponent<Billboarding>();
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// Sketch plane is constantly facing a particular user (aka Billboarding)
    /// </summary>
    /// <param name="sketch">The Sketch GameObject to place interaction on</param>
    public void BillboardingIndividual(GameObject sketch)
    {
        throw new System.NotImplementedException("Once PaperPic is added to the Server this can be implemented.");
        /*GameObject sketch = GeneratePlane();
        sketches.Add(sketch);
        sketch.tag = "SKETCH_BillboardAll";
        sketch.AddComponent<Billboarding>().Target = player.gameObject;*/
    }

    /// <summary>
    /// Creates a GameObject and attaches a 3D Sound to it.
    /// Adds the tag "SKETCH_LoopingSound"
    /// If told to attach it to a Sketch, will create a SketchPlane and attach the sound to that
    /// </summary>
    /// <param name="attachToSketch">Whether or not to download a sketch and attach the sound to the sketch</param>
    /// <param name="soundPath">The Resource path to the audio file (do not add file ending)</param>
    public void LoopingSound(GameObject sketch, string soundPath)
    {
        tag = "SKETCH_LoopingSound";
        CreateAudioObject(sketch, tag, (soundPath == null) ? "Sounds/Granulizer" : soundPath, 0);
        AudioSource aud = sketch.GetComponent<AudioSource>();
        aud.loop = true;
        aud.Play();
    }

    /// <summary>
    /// Sound plays when user gets near SketchPlane
    /// </summary>
    public void ProximitySound(GameObject sketch, string soundPath, float radius)
    {
        string tag = "SKETCH_SoundOnCollision";
        //GameObject collisionObject = ColliderObjectSetup(sketch, tag);

        CreateAudioObject(sketch, tag, (soundPath==null) ? "Sounds/GlassBreaking":soundPath, radius);

        AudioSource aud = sketch.GetComponent<AudioSource>();
        aud.loop = false;
    }

    /// <summary>
    /// Sound stops playing when user gets near SketchPlane
    /// </summary>
    public void ProximitySilence(GameObject sketch, string soundPath, float radius)
    {
        string tag = "SKETCH_SoundOffOnCollision";
        //GameObject collisionObject = ColliderObjectSetup(sketch, tag);

        CreateAudioObject(sketch, tag, (soundPath == null) ? "Sounds/BabyCrying" : soundPath, radius);

        AudioSource aud = sketch.GetComponent<AudioSource>();
        aud.loop = true;
        aud.Play();
    }

    /// <summary>
    /// Object moves about the space between points determined by the user
    /// An interface appears as a result of this function
    /// </summary>
    /// <param name="speed">The speed at which the sketch will move</param>
    public void ProximityTranslate(GameObject sketch, float radius, float speed)
    {
        GameObject collisionObject = ColliderObjectSetup(sketch, "SKETCH_ProximityTranslation");
        collisionObject.AddComponent<SphereCollider>().isTrigger = true;
        collisionObject.GetComponent<SphereCollider>().radius = radius;
        collisionObject.SetActive(false);

        sketch.SetActive(false); //set inactive so variables can be set without start() being called

        TranslationInteractionManager transManager = sketch.AddComponent<TranslationInteractionManager>();
        transManager.Template = TranslationPointPrefab;
        transManager.translationSpeed = speed;

        sketch.SetActive(true);
    }


    //// ADD MORE INTERACTIONS HERE ////
#endregion Interactions

    #region Helper


    /// <summary>
    /// Place passed object in front of main camera
    /// FIXME: Copied from ImageDownloader
    /// </summary>
    /// <param name="obj"></param>
    void placeObject(GameObject obj)
    {
        float dist = 0.5f;
        Vector3 offset = new Vector3(0, -0.05f, 0);
        Vector3 newPos = Camera.main.transform.position + Camera.main.transform.forward;
        newPos *= dist;
        newPos += offset;

        obj.transform.position = newPos;
        if (obj.GetComponent<ImageDownloader>())
        {
            obj.GetComponent<ImageDownloader>().SendTransform();
        }
    }

    /// <summary>
    /// Method to create a new SketchPlane object
    /// creates the new SketchPlane object and sets its url
    /// </summary>
    GameObject GeneratePlane()
    {
        // Generate new SketchPlane object and set its url
        sketchPlane.SetActive(true);
        GameObject newSketch = Instantiate(sketchPlane, transform);
        sketchPlane.SetActive(false);
        numSketchPlanes++;
        ImageDownloader newSketchDownloader = newSketch.GetComponentInChildren<ImageDownloader>();
        //TODO: add check to make sure index is available and won't create a 404
        newSketchDownloader.baseUrl = baseUrl;
        newSketchDownloader.Id = numSketchPlanes;
        StartCoroutine(newSketchDownloader.UpdateTexture(true));

        sketches.Add(newSketch);
        return newSketch;
    } // Delete this in the future. Only for hard coded key presses.

    /// <summary>
    /// Method to create a new SketchPlane object
    /// creates the new SketchPlane object and sets its url
    /// </summary>
    /// <param name="sketchID">The ID number for the sketch to generate</param>
    GameObject GeneratePlane(int sketchID, int instanceIndex, bool updateLocation = true)
    {
        // Generate new SketchPlane object and set its url
        sketchPlane.SetActive(true);
        GameObject newSketch = Instantiate(sketchPlane, transform);
        sketchPlane.SetActive(false);

        //keeping track of all the sketches
        ImageDownloader newSketchDownloader = newSketch.GetComponentInChildren<ImageDownloader>();

        newSketchDownloader.baseUrl = baseUrl;
        newSketchDownloader.Id = sketchID;
        newSketchDownloader.instanceIndex = instanceIndex;
        StartCoroutine(newSketchDownloader.UpdateTexture(updateLocation));
        //StartCoroutine(newSketchDownloader.getInteractions());

        sketches.Add(newSketch);
        return newSketch;
    }

    /// <summary>
    /// Method to create a new SketchPlane object
    /// creates the new SketchPlane object and sets its url
    /// </summary>
    /// <param name="url">the full url of the image to place on plane</param>
    /// <returns></returns>
    GameObject GeneratePlane(string url)
    {
        sketchPlane.SetActive(true);
        GameObject newSketch = Instantiate(sketchPlane, transform);
        sketchPlane.SetActive(false);

        ImageDownloader newSketchDownloader = newSketch.GetComponentInChildren<ImageDownloader>();

        newSketchDownloader.SetUrl(url);
        StartCoroutine(newSketchDownloader.UpdateTexture(true));

        sketches.Add(newSketch);
        return newSketch;
    }

    /// <summary>
    /// Simple Helper function which creates a Sketch Plane GameObject
    /// and a Child of it called "Collider".
    /// gives Collider a tag
    /// and places it in the layer which ignores RayCasts
    /// so that the object isn't accidentally moved because it's collider
    /// of unknown size is selected.
    /// </summary>
    /// <param name="interactionTag">The GameObject tag to place on the object with Collider</param>
    /// <returns></returns>
    GameObject ColliderObjectSetup(GameObject sketch, string interactionTag)
    {
        GameObject collisionObject = new GameObject("Collider");
        GameObject colObj = Instantiate(collisionObject, sketch.transform);
        Destroy(collisionObject);

        colObj.tag = interactionTag;
        colObj.layer = 2;

        colObj.AddComponent<TriggerHandler>();

        return colObj;
    }

    /// <summary>
    /// Gives SketchPlane an AudioSource component
    /// and adds an interaction tag
    /// Also gives it a sphere collider
    /// </summary>
    /// <param name="interactionTag">The Tag to add to the audio object or collider</param>
    /// <returns></returns>
    GameObject CreateAudioObject(GameObject attachedTo, string interactionTag, string pathToSound, float radius)
    {
        attachedTo.AddComponent<AudioSource>().spatialBlend = 1;
        attachedTo.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(pathToSound);

        if (radius != 0.0)
        {
            GameObject obj = new GameObject("Collider");
            GameObject collider = Instantiate(obj, attachedTo.transform);
            Destroy(obj);

            collider.AddComponent<TriggerHandler>();
            collider.AddComponent<SphereCollider>().radius = radius;
            collider.GetComponent<SphereCollider>().isTrigger = true;
            collider.layer = 2;
            collider.tag = interactionTag;
        }

        return attachedTo;
    }

    #endregion Helper

    #region delete / reset
    /// <summary>
    /// Method to delete sketches which are erased
    /// updates list of sketches
    /// (Doesn't need to update urls for other 
    /// </summary>
    /// <param name="plane">the SketchPlane GameObject to destroy</param>
    public void DeletePlane(GameObject plane, bool tellServer = true)
    {
        GameObject[] before = sketches.ToArray();
        int id = plane.GetComponent<ImageDownloader>().Id;
        if (plane.GetType() != sketchPlane.GetType())
        {
            throw new System.ArgumentException(string.Format("method DeletePlane in ImageGenerator got wrong GameObject passed.\n\tgot {0}\n\tneeded {1}", plane.GetType(), sketchPlane.GetType()));
        }

        //if (tellServer)
          //  plane.GetComponent<ImageDownloader>().SendDeletion();

        // Remove from list of sketches
        sketches.Remove(plane);
        // Remove Bounding Box
        //if (plane.GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().boundingBoxInstance)
        //{
        //    Destroy(plane.GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().boundingBoxInstance.gameObject);
        //}
        // Destroy object
        Destroy(plane);

        GameObject[] after = sketches.ToArray();
        Debug.Log(string.Format("Deleted Sketch Plane with Id {0}. Before Deletion, sketches had {1} sketches. Now it has {2}.", id, before.Length, after.Length));
    }
    public GameObject DeleteInteractions(GameObject plane)
    {
        int id = plane.GetComponent<ImageDownloader>().Id;
        int inst = plane.GetComponent<ImageDownloader>().instanceIndex;
        int[] frames = plane.GetComponent<ImageDownloader>().framesActive;
        Transform transform = plane.transform;

        // Replace Plane in sketches list
        int indexInSketches = sketches.IndexOf(plane);
        GameObject newPlane = GeneratePlane(id, inst, false);
        if (indexInSketches != -1)
        {
            sketches[indexInSketches] = newPlane;
        }

        // Destroy Bounding Box if needed
        //if (plane.GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().boundingBoxInstance)
        //{
        //    Destroy(plane.GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().boundingBoxInstance.gameObject);
        //}
        // Destroy object
        Destroy(plane);

        //DeletePlane(plane, false);
        //plane = GeneratePlane(id, inst, false);
        newPlane.transform.SetPositionAndRotation(transform.position, transform.rotation);
        newPlane.transform.localScale = transform.localScale;
        newPlane.GetComponent<ImageDownloader>().framesActive = frames;

        return newPlane;
    }
    public void SaveTranslationData(GameObject plane)
    {

    }
    public void ResetInteractions(GameObject plane, InteractionsJson interactions, bool resetTranslate)
    {
        if (resetTranslate)
        {
            plane = DeleteInteractions(plane);
            getInteractionsFromJson(interactions, plane);
        }
        else
        {
            Debug.Log("Keeping Translation is not implemented.");

            // Get old Translation Data
            TranslationData transData;
            if (plane.GetComponent<TranslationInteractionManager>())
            {
                transData = plane.GetComponent<TranslationInteractionManager>().ExportData();
            }

            plane = DeleteInteractions(plane);
            getInteractionsFromJson(interactions, plane);
        }
    }
    #endregion
}






#region JSON Objects
[System.Serializable]
public class SketchJson
{
    public int sketchId;
    public int instanceIndex;
    public string type;
    public int[] frames;
    public InteractionsJson interactions;
}
[System.Serializable]
public class InteractionsJson
{
    public bool disappear;
    public bool disappear_proximity;
    public float disappear_proximity_radius;
    public bool disappear_gaze;
    public float disappear_gaze_radius;

    public bool appear;
    public bool appear_proximity;
    public float appear_proximity_radius;
    public bool appear_gaze;
    public float appear_gaze_radius;

    public bool sound;
    public bool sound_loop;
    public string sound_loop_file;
    public bool sound_approach;
    public string sound_approach_file;
    public float sound_approach_radius;
    public bool sound_leave;
    public string sound_leave_file;
    public float sound_leave_radius;

    public bool billboard;

    public bool translate;
    public float translate_radius;
    public float translate_speed;

    new public string ToString()
    {
        string str = "JSON: {";

        str += string.Format("\tdisappear_proximity : {0}\n", disappear_proximity);
        str += string.Format("\tdisappear_proximity_radius : {0}\n", disappear_proximity_radius);
        str += string.Format("\tdisappear_gaze : {0}\n", disappear_gaze);
        str += string.Format("\tdisappear_gaze_radius : {0}\n", disappear_gaze_radius);

        str += string.Format("\tappear_proximity : {0}\n", appear_proximity);
        str += string.Format("\tappear_proximity_radius : {0}\n", appear_proximity_radius);
        str += string.Format("\tappear_gaze : {0}\n", appear_gaze);
        str += string.Format("\tappear_gaze_radius : {0}\n", appear_gaze_radius);

        str += string.Format("\tsound_loop : {0}\n", sound_loop);
        str += string.Format("\tsound_loop_file : {0}\n", sound_loop_file);
        str += string.Format("\tsound_approach : {0}\n", sound_approach);
        str += string.Format("\tsound_approach_file : {0}\n", sound_approach_file);
        str += string.Format("\tsound_approach_radius : {0}\n", sound_approach_radius);
        str += string.Format("\tsound_leave : {0}\n", sound_leave);
        str += string.Format("\tsound_leave_file : {0}\n", sound_leave_file);
        str += string.Format("\tsound_leave_radius : {0}\n", sound_leave_radius);

        str += string.Format("\tbillboard : {0}\n", billboard);

        str += string.Format("\ttranslate : {0}\n", translate);
        str += string.Format("\ttranslate_radius : {0}\n", translate_radius);
        str += string.Format("\ttranslate_speed : {0}\n", translate_speed);

        str += "}";
        return str;
    }
}

[System.Serializable]
public class ExternalLinkJson
{
    public string type;

    public string url;
}

[System.Serializable]
public class RecordJson
{
    public string type;
    public bool start;
}

[System.Serializable]
public class EditJson
{
    public string type;
    public bool status;
}

[System.Serializable]
public class UpdateJson
{
    public string type;
    public int sketchId;
    public int instanceIndex;
    public InteractionsJson interactions;
    public bool updateTranslation;
}

[System.Serializable]
public class FrameJson
{
    public string type;
    public int activeFrame;
}

[System.Serializable]
public class TriggerJson
{
    public string type;
    public int sketchId;
    public int instanceIndex;
    public string trigger;
}

[System.Serializable]
public class DeleteJson
{
    public string type;
    public int sketchId;
    public string instanceIndex;
}

[System.Serializable]
public class BulkTransformUpdateJson
{
    public string type;
    public TransformUpdateJson[] updates;
}

#endregion
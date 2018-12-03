using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerHandler : MonoBehaviour {
    private static bool _EditMode;
    public static bool EditMode
    {
        get
        {
            return TriggerHandler._EditMode;
        }
        set
        {
            // Set Value
            TriggerHandler._EditMode = value;

            // Make all invisible sketches visible - for the current frame
            List<GameObject> sketches = GameObject.FindGameObjectWithTag("SketchManager").GetComponent<ImageGenerator>().sketches;
            int currentFrame = GameObject.FindGameObjectWithTag("SketchManager").GetComponent<ImageGenerator>().currentFrame;

            foreach (GameObject s in sketches)
            {
                ImageDownloader dl = s.GetComponent<ImageDownloader>();
                int[] frames = dl.framesActive;

                // Check if instance is active in current frame
                if (System.Array.IndexOf(frames, currentFrame) != -1)
                {
                    // Set instance visible
                    s.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }
    /// <summary>
    /// Triggers based on collider triggers
    /// </summary>
    #region Proximity

    /// <summary>
    /// When Camera enters a collider space
    /// </summary>
    /// <param name="other">Information about the collider which the camera entered</param>
    void OnTriggerEnter(Collider other)
    {
        if (!EditMode)
        {
            Debug.Log(string.Format("{0} was triggered by {1} ({2} tag)", tag, other.name, other.tag));

            if (other.tag.Equals("MainCamera"))
            {
                switch (tag)
                {
                    case "SKETCH_DisappearOnCollision":
                    case "SKETCH_DisappearWhenLookedAt":
                        GetComponentInParent<MeshRenderer>().enabled = false;
                        GetComponentInParent<MeshCollider>().enabled = false;
                        break;
                    case "SKETCH_AppearOnCollision":
                    case "SKETCH_AppearWhenLookedAt":
                        GetComponentInParent<MeshRenderer>().enabled = true;
                        GetComponentInParent<MeshCollider>().enabled = true;
                        break;
                    case "SKETCH_BillboardAll":
                        // Do nothing - script is contained in Sketch Plane object
                        break;
                    case "SKETCH_SoundOnCollision":
                        GetComponentInParent<AudioSource>().Play();
                        break;
                    case "SKETCH_SoundOffOnCollision":
                        GetComponentInParent<AudioSource>().Pause();
                        break;
                    case "SKETCH_ProximityTranslation":
                        TranslationInteractionManager translationManager = GetComponentInParent<TranslationInteractionManager>();
                        if (!translationManager.moving && !translationManager.isBeingDragged)
                        {
                            Debug.Log("Calling Translate");
                            // only make call if it isn't already moving
                            translationManager.Translate();
                        }
                        else
                        {
                            Debug.Log(string.Format("Dragged: {0}\nMoving: {1}", translationManager.isBeingDragged, translationManager.moving));
                        }
                        break;

                        // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
                }
            }
            else
            {
                Debug.Log(string.Format("Collided with ", other.tag));
            }
        }
    }

    /// <summary>
    /// While Camera is in a collider space
    /// </summary>
    /// <param name="other">Information about the collider which the camera is in</param>
    void OnTriggerStay(Collider other)
    {
        if (!_EditMode)
        {

            // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
        }
    }

    /// <summary>
    /// When Camera leaves a collider space
    /// </summary>
    /// <param name="other">Information about the collider which camera left</param>
    void OnTriggerExit(Collider other)
    {
        if (!_EditMode)
        {
            //Debug.Log(string.Format("{0} trigger with {1} was ended", tag, other.tag));
            if (other.tag.Equals("MainCamera"))
            {
                switch (tag)
                {
                    case "SKETCH_DisappearOnCollision":
                    case "SKETCH_DisappearWhenLookedAt":
                        GetComponentInParent<MeshRenderer>().enabled = true;
                        GetComponentInParent<MeshCollider>().enabled = true;
                        break;
                    case "SKETCH_AppearOnCollision":
                    case "SKETCH_AppearWhenLookedAt":
                        GetComponentInParent<MeshRenderer>().enabled = false;
                        GetComponentInParent<MeshCollider>().enabled = false;
                        break;
                    case "SKETCH_SoundOnCollision":
                        //other.gameObject.GetComponentInParent<AudioSource>().Stop(); //Don't stop until you get enough. Play the whole sound even if it's a hit and run.
                        break;
                    case "SKETCH_SoundOffOnCollision":
                        GetComponentInParent<AudioSource>().UnPause();
                        break;
                    case "SKETCH_ProximityTranslate":
                        break; // let translation finish

                        // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
                }
            }
        }
    }

    #endregion Proximity


    /// <summary>
    /// Triggers based on gaze events
    /// </summary>
    #region Gaze

    public void OnFocusStarted()
    {
        if (!_EditMode)
        {
            switch (tag)
            {
                case "SKETCH_DisappearWhenLookedAt":
                    GetComponentInParent<MeshRenderer>().enabled = false;
                    GetComponentInParent<MeshCollider>().enabled = false;
                    break;
                case "SKETCH_AppearWhenLookedAt":
                    GetComponentInParent<MeshRenderer>().enabled = true;
                    GetComponentInParent<MeshCollider>().enabled = true;
                    break;
                case "SKETCH_DisappearOnCollision":
                case "SKETCH_AppearOnCollision":
                case "SKETCH_BillboardAll":
                case "SKETCH_SoundOnCollision":
                case "SKETCH_SoundOffOnCollision":
                case "SKETCH_ProximityTranslation":
                default:
                    break;

                    // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
            }
        }
    }

    public void OnFocusContinued()
    {
        if (!_EditMode)
        {
            switch (tag)
            {
                default:
                    return;

                    // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
            }
        }
    }

    public void OnFocusEnded()
    {
        if (!_EditMode)
        {
            switch (tag)
            {
                case "SKETCH_DisappearWhenLookedAt":
                    GetComponentInParent<MeshRenderer>().enabled = true;
                    GetComponentInParent<MeshCollider>().enabled = true;
                    break;
                case "SKETCH_AppearWhenLookedAt":
                    GetComponentInParent<MeshRenderer>().enabled = false;
                    GetComponentInParent<MeshCollider>().enabled = false;
                    break;
                case "SKETCH_DisappearOnCollision":
                case "SKETCH_AppearOnCollision":
                case "SKETCH_SoundOnCollision":
                case "SKETCH_SoundOffOnCollision":
                case "SKETCH_ProximityTranslate":
                default:
                    break;

                    // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
            }
        }
    }

    #endregion Gaze


    public void ManualTrigger(string type)
    {
        Debug.Log("Forced Trigger of " + tag);
        switch (type)
        {
            case "disappear_proximity":
            case "disappear_gaze":
                GetComponentInParent<MeshRenderer>().enabled = false;
                GetComponentInParent<MeshCollider>().enabled = false;
                break;
            case "appear_proximity":
            case "appear_gaze":
                GetComponentInParent<MeshRenderer>().enabled = true;
                GetComponentInParent<MeshCollider>().enabled = true;
                break;
            case "billboard":
                // Do nothing - script is contained in Sketch Plane object
                break;
            case "sound_approach":
                GetComponentInParent<AudioSource>().Play();
                break;
            case "sound_leave":
                GetComponentInParent<AudioSource>().Pause();
                break;
            case "translate":
                TranslationInteractionManager translationManager = GetComponentInParent<TranslationInteractionManager>();
                if (!translationManager.moving && !translationManager.isBeingDragged)
                {
                    Debug.Log("Calling Translate");
                    // only make call if it isn't already moving
                    translationManager.Translate();
                }
                else
                {
                    Debug.Log(string.Format("Dragged: {0}\nMoving: {1}", translationManager.isBeingDragged, translationManager.moving));
                }
                break;
            default:
                Debug.Log("Unknown Manual Trigger type" + type);
                break;

                // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
        }

        // TODO: How to reset later
        //var task = System.Threading.Tasks.Task.Run(() => Detrigger(type));
        //task.Wait(System.Threading.Tasks.TimeSpan.FromSeconds(20));
    }

    public void Detrigger(string type)
    {
        switch (type)
        {
            case "disappear_proximity":
            case "disappear_gaze":
                GetComponentInParent<MeshRenderer>().enabled = true;
                GetComponentInParent<MeshCollider>().enabled = true;
                break;
            case "appear_proximity":
            case "appear_gaze":
                GetComponentInParent<MeshRenderer>().enabled = false;
                GetComponentInParent<MeshCollider>().enabled = false;
                break;
            case "sound_approach":
                //other.gameObject.GetComponentInParent<AudioSource>().Stop(); //Don't stop until you get enough. Play the whole sound even if it's a hit and run.
                break;
            case "sound_leave":
                GetComponentInParent<AudioSource>().UnPause();
                break;
            case "translate":
                break; // let translation finish

                // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
        }
    }
}

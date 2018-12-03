using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerForSketches : MonoBehaviour {

    private GameObject ObjLookedAtLastFrame = null;



    /// <summary>
    /// Constantly checking Gaze to see if should be triggering interactions.
    /// </summary>
	void Update () {
        GameObject HitObject = HoloToolkit.Unity.InputModule.GazeManager.Instance.HitObject;

        // Check if Gaze hits a Sketch Object
        if (HitObject != null && HitObject.tag.StartsWith("SKETCH_"))
        {
            if (ObjLookedAtLastFrame == null)
            { 
                // If looking at a new Sketch Plane Collider:
                OnTriggerEnter(HitObject.GetComponent<Collider>());
                ObjLookedAtLastFrame = HitObject;
            }
            else if (ReferenceEquals(ObjLookedAtLastFrame, HitObject))
            { 
                // If still looking at same Sketch Plane Collider:
                OnTriggerStay(HitObject.GetComponent<Collider>());
            }
            else if (!ReferenceEquals(ObjLookedAtLastFrame, HitObject)) 
            {
                // If suddenly looking at new Sketch Plane Collider:
                OnTriggerExit(ObjLookedAtLastFrame.GetComponent<Collider>());
                OnTriggerEnter(HitObject.GetComponent<Collider>());
                ObjLookedAtLastFrame = null;
            }

            ObjLookedAtLastFrame = HitObject;
        }
        else if (ObjLookedAtLastFrame != null)
        { // If stopped looking at Sketch Plane Collider
            OnTriggerExit(ObjLookedAtLastFrame.GetComponent<Collider>());
            ObjLookedAtLastFrame = null;
        }

	}


    #region Triggers

    /// <summary>
    /// When Camera enters a collider space
    /// </summary>
    /// <param name="other">Information about the collider which the camera entered</param>
    void OnTriggerEnter(Collider other)
    {
        print("Camera hit collider");
        switch(other.gameObject.tag)
        {
            case "SKETCH_DisappearOnCollision":
            case "SKETCH_DisappearWhenLookedAt":
                other.gameObject.GetComponentInParent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponentInParent<MeshCollider>().enabled = false;
                break;
            case "SKETCH_AppearOnCollision":
            case "SKETCH_AppearWhenLookedAt":
                other.gameObject.GetComponentInParent<MeshRenderer>().enabled = true;
                other.gameObject.GetComponentInParent<MeshCollider>().enabled = true;
                break;
            case "SKETCH_BillboardAll":
                // Do nothing - script is contained in Sketch Plane object
                break;
            case "SKETCH_SoundOnCollision":
                other.gameObject.GetComponentInParent<AudioSource>().Play();
                break;
            case "SKETCH_SoundOffOnCollision":
                other.gameObject.GetComponentInParent<AudioSource>().Pause();
                break;
            case "SKETCH_ProximityTranslation":
                print("Translate");
                TranslationInteractionManager translationManager = other.gameObject.GetComponentInParent<TranslationInteractionManager>();
                if (!translationManager.moving && !translationManager.isBeingDragged)
                {
                    // only make call if it isn't already moving
                    translationManager.Translate();
                }
                break;

                // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
        }
    }

    /// <summary>
    /// While Camera is in a collider space
    /// </summary>
    /// <param name="other">Information about the collider which the camera is in</param>
    void OnTriggerStay(Collider other)
    {
        print("Trigger Stay");
        // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
    }

    /// <summary>
    /// When Camera leaves a collider space
    /// </summary>
    /// <param name="other">Information about the collider which camera left</param>
    void OnTriggerExit(Collider other)
    {
        print("Trigger Exit");
        switch (other.gameObject.tag)
        {
            case "SKETCH_DisappearOnCollision":
            case "SKETCH_DisappearWhenLookedAt":
                other.gameObject.GetComponentInParent<MeshRenderer>().enabled = true;
                other.gameObject.GetComponentInParent<MeshCollider>().enabled = true;
                break;
            case "SKETCH_AppearOnCollision":
            case "SKETCH_AppearWhenLookedAt":
                other.gameObject.GetComponentInParent<MeshRenderer>().enabled = false;
                other.gameObject.GetComponentInParent<MeshCollider>().enabled = false;
                break;
            case "SKETCH_SoundOnCollision":
                //other.gameObject.GetComponentInParent<AudioSource>().Stop(); //Don't stop until you get enough. Play the whole sound even if it's a hit and run.
                break;
            case "SKETCH_SoundOffOnCollision":
                other.gameObject.GetComponentInParent<AudioSource>().UnPause();
                break;
            case "SKETCH_ProximityTranslate":
                break; // let translation finish

                // ADD MORE COLLIDER TAGS HERE ALONG WITH FUNCTIONALITY
        }
    }

    #endregion Triggers
}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FocusEventManager : MonoBehaviour
{

    public event Action<GameObject, GameObject> GazeStarted;
    public event Action<GameObject, GameObject> GazeEnded;
    public event Action<GameObject, GameObject> GazeContinued;
    public event Action<GameObject, GameObject, GameObject> GazeChanged;

    public bool isLookingAtObject { get; private set; }
    private GameObject lastObject;
    private GameObject currentObject;

    /// <summary>
    /// Setup a few methods to run on actions
    /// Send collisions to SketchPlanes if gazed at by "MainCamera"
    /// </summary>
    private void Start()
    {
        GazeStarted += SendTriggerEnter;
        GazeContinued += SendTriggerStay;
        GazeEnded += SendTriggerExit;
    }

    /// <summary>
    /// Constantly check Gaze Manager to see if gaze has changed
    /// Raise events (GazeStarted, GazeEnded, GazeContinued, and GazeChanged)
    /// in appropriate conditions
    /// </summary>
	void Update()
    {
        currentObject = HoloToolkit.Unity.InputModule.GazeManager.Instance.HitObject;
        GameObject gazeSource = HoloToolkit.Unity.InputModule.GazeManager.Instance.gameObject;

        if (currentObject == null)
        { // If no longer looking at anything
            if (lastObject != null)
            {
                // GazeEnded event if was previously looking at an object
                GazeEnded.Invoke(gazeSource, lastObject);
            }
            isLookingAtObject = false;
        }
        else if (lastObject == null)
        { // If looking at something after looking at nothing
            GazeStarted.Invoke(gazeSource, currentObject);
            isLookingAtObject = true;
        }
        else if (lastObject != null)
        { // If looking at something after already looking at something
            if (lastObject == currentObject)
            { // if looking at the same object
                GazeContinued.Invoke(gazeSource, currentObject);
            }
            else
            { // if looking at a new object
                GazeEnded.Invoke(gazeSource, lastObject);
                GazeStarted.Invoke(gazeSource, currentObject);
                //GazeChanged(gazeSource, lastObject, currentObject);
            }
        }

        lastObject = currentObject;
    }


    /// <summary>
    /// If one of the SketchPlanes is being looked at
    /// send it the on trigger message.
    /// Also check to make sure it is the MainCamera
    /// which is doing the looking
    /// </summary>
    /// <param name="focusSource">The Camera which is doing the looking</param>
    /// <param name="focusedObject">The object being looked at</param>
    #region Action Methods

    void SendTriggerEnter(GameObject focusSource, GameObject focusedObject)
    {
        TriggerHandler handler = focusedObject.GetComponent<TriggerHandler>();
        //print(string.Format("{0} '{1}' is looking at {2}", focusSource.tag, focusSource.name, focusedObject.tag));

        if (handler != null && focusSource.name == "InputManager") //tag == "MainCamera"
        {
            handler.OnFocusStarted();
        }
    }

    void SendTriggerStay(GameObject focusSource, GameObject focusedObject)
    {
        TriggerHandler handler = focusedObject.GetComponent<TriggerHandler>();
        if (handler != null && focusSource.name == "InputManager")
        {
            handler.OnFocusContinued();
        }
    }

    void SendTriggerExit(GameObject focusSource, GameObject focusedObject)
    {
        TriggerHandler handler = focusedObject.GetComponent<TriggerHandler>();
        if (handler != null && focusSource.name == "InputManager")
        {
            handler.OnFocusEnded();
        }
    }

    #endregion Action Methods
}

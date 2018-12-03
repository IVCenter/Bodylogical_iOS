using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


public class TranslationPointManager : MonoBehaviour
{
    public int index;
    private Vector3 startLoc;
    private bool dragging;
    private float CLOSE_LOOP_DIST = 0.1f;

    /// <summary>
    /// Adds Methods to trigger when object is dragged
    /// </summary>
    private void Start()
    {
        startLoc = transform.position;
        GetComponent<HandDraggable>().StoppedDragging += TranslationPointManager_StoppedDragging;
        GetComponent<HandDraggable>().StartedDragging += TranslationPointManager_StartedDragging;
    }

    /// <summary>
    /// Keeps object's orientation constant
    /// and calls whileDragged if being dragged
    /// </summary>
    private void Update()
    {
        if (dragging) { whileDragged(); }

        // Prevent rotations
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(new Vector3(90, 180, 0));
    }

    #region Event Based
    /// <summary>
    /// Sets object text to appropriate number
    /// Or calls EndSetup() depending on object location
    /// Triggered when object is placed somewhere
    /// </summary>
    private void TranslationPointManager_StoppedDragging()
    {
        GetComponent<TextMesh>().text = index.ToString();

        if (Vector3.Distance(transform.position, startLoc) < CLOSE_LOOP_DIST)
        {
            GetComponent<TextMesh>().text = "END";
            GetComponentInParent<TranslationInteractionManager>().EndSetup(gameObject);
        }
        else { GetComponent<TextMesh>().text = index.ToString(); }

        dragging = false;
        //print(string.Format("Dropped at: {0}", transform.position));
    }

    /// <summary>
    /// Triggered when object is first picked up for dragging
    /// </summary>
    private void TranslationPointManager_StartedDragging()
    {
        dragging = true;
    }

    /// <summary>
    /// Sets the text of the object while it is being dragged
    /// 'END' if dropping the object will trigger EndSetup()
    /// its index if it won't.
    /// </summary>
    private void whileDragged()
    {
        if (Vector3.Distance(transform.position, startLoc) < CLOSE_LOOP_DIST)
        {
            GetComponent<TextMesh>().text = "END";
        }
        else
        {
            GetComponent<TextMesh>().text = index.ToString();
        }
    }
    #endregion Event Based
}



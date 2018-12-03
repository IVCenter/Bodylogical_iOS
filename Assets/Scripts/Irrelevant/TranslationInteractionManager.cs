using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;


public class TranslationInteractionManager : MonoBehaviour {
    #region Variables
    #region Public

    /// <summary>The GameObject to use for each point during translation setup</summary>
    public GameObject Template;
    /// <summary>The speed at which sketch will move along the translation path</summary>
    public float translationSpeed;
    /// <summary>Event invoked when translation has finished being setup</summary>
    //public event System.Action SetupComplete;

    #endregion Public


    #region Public Get, Private Set

    /// <summary>Whether the sketch is currently translating</summary>
    public bool moving { get; private set; }
    ///<summary>Whether translation has completed setup</summary>
    public bool SetupCompleted { get; private set; }
    ///<summary>Whether HandDraggable component is dragging the gameObject</summary>
    public bool isBeingDragged { get; private set; }
    ///<summary>Whether a point in the translation path is currently being dragged</summary>
    public bool pointIsBeingMoved { get; private set; }
    #endregion Public Get, Private Set


    #region Private

    ///<summary>point GameObjects along the translation path</summary>
    private List<GameObject> points = new List<GameObject>();
    ///<summary>Location which sketch is headed</summary>
    private Vector3 target;

    ///<summary>Location of each point along translation relative to the sketch</summary>
    private Vector3[] relativePointPositions;
    /// <summary>Location of each point along translation</summary>
    private Vector3[] absolutePointPositions;
    /// <summary>the starting state - whether rendered or not (determined by other interactions)</summary>
    private bool initialRenderState;

    #endregion Private
    #endregion Variables

    #region Methods

    /// <summary>
    /// Creates first point and sets up methods
    /// which are called when point is placed
    /// </summary>
    void Start() {
        points.Add(gameObject);
        generatePoint();
        HandDraggable newPoint = points[points.Count - 1].GetComponent<HandDraggable>();
        newPoint.StoppedDragging += TranslationInteractionManager_StoppedDraggingInit;
        newPoint.StartedDragging += TranslationInteractionManager_StartedDraggingInit;
        //newPoint.StoppedDragging += updateLineBetween;
        newPoint.StartedDragging += StartedDragging;
        newPoint.StoppedDragging += StoppedDragging;

        // Set Sketch HandDraggable actions
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StartedManipulation += SketchDragStarted;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StoppedManipulation += SketchDropped;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().StoppedManipulation += UpdateVarsOnSketchDragged;
    }
    /// <summary>
    /// Updates translate
    /// </summary>
    void Update()
    {
        if (!SetupCompleted) { }

        if (isBeingDragged)
        {
            moving = false;
            //updateLineBetween();
            if (!SetupCompleted)
                updateLineBetween();
        }
        else if (moving)
            Translate();
        else if (Input.GetKeyDown(KeyCode.T))
            Translate();

        if (pointIsBeingMoved) updateLineBetween();
    }


    #region Event Based Methods
    #region For Point Movement

    private void TranslationInteractionManager_StartedDraggingInit()
    {
        Debug.Log("started drag");
        HandDraggable newPoint = points[points.Count - 1].GetComponent<HandDraggable>();
        HandDraggable oldPoint = points[points.Count - 2].GetComponent<HandDraggable>();
        drawLineBetween(oldPoint.gameObject, newPoint.gameObject);
        Debug.Log("Drew Line");
    }

    private void StartedDragging()
    {
        pointIsBeingMoved = true;
    }

    private void TranslationInteractionManager_StoppedDraggingInit()
    {
        print("TranslationInteractionManager: Point was dropped");
        generatePoint();
        HandDraggable newPoint = points[points.Count - 1].GetComponent<HandDraggable>();
        HandDraggable oldPoint = points[points.Count - 2].GetComponent<HandDraggable>();
        newPoint.StoppedDragging += TranslationInteractionManager_StoppedDraggingInit;
        newPoint.StartedDragging += TranslationInteractionManager_StartedDraggingInit;
        //newPoint.StoppedDragging += updateLineBetween;
        newPoint.StartedDragging += StartedDragging;
        newPoint.StoppedDragging += StoppedDragging;
        oldPoint.StoppedDragging -= TranslationInteractionManager_StoppedDraggingInit;
        oldPoint.StartedDragging -= TranslationInteractionManager_StartedDraggingInit;

        print(string.Format("Added TranslationInteractionManager_StoppedDragging to {0} and removed it from {1}", newPoint.gameObject.GetComponent<TextMesh>().text, oldPoint.gameObject.GetComponent<TextMesh>().text));
    }

    private void StoppedDragging()
    {
        pointIsBeingMoved = false;
    }

    private void updateLineBetween()
    {
        LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();

        if (points.Count > 1)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].SetPosition(0, points[i].transform.position);
                lines[i].SetPosition(1, points[i + 1].transform.position);
            }
        }
    }
    #endregion For Point Movement

    #region For Sketch Movement

    /// <summary>
    /// Immediately returns sketch to its starting location (homes it)
    /// and ends any movement
    /// </summary>
    public void HaltTranslation()
    {
        moving = false;
        if (SetupCompleted)
        {
            transform.position = absolutePointPositions[0];
            target = absolutePointPositions[1];
        }
    }

    private void SketchDragStarted()
    {
        isBeingDragged = true;
        HaltTranslation();

        if (!SetupCompleted)
        {
            SphereCollider[] children = GetComponentsInChildren<SphereCollider>();
            foreach (SphereCollider child in children)
            {
                child.enabled = false;
            }
        }
    }

    private void SketchDropped()
    {
        isBeingDragged = false;
        if (SetupCompleted)
        {
            LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer line in lines)
            {
                line.gameObject.SetActive(false);
            }
        }
        else
        {
            SphereCollider[] children = GetComponentsInChildren<SphereCollider>();
            foreach (SphereCollider child in children)
            {
                child.enabled = true;
            }
        }
    }

    /// <summary>
    /// Keeps all translation points relative to sketch
    /// Ends movement and homes sketch if currently moving
    /// </summary>
    private void UpdateVarsOnSketchDragged()
    {
        absolutePointPositions[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    #endregion For Sketch Movement
    #endregion Event Based Methods


    public void EndSetup(GameObject objToRemove)
    {
        SetupCompleted = true;
        LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();

        // Hide all elements
        lines = GetComponentsInChildren<LineRenderer>();
        TextMesh[] numbers = GetComponentsInChildren<TextMesh>();
        foreach (LineRenderer line in lines) { line.gameObject.SetActive(false); }
        foreach (TextMesh num in numbers) { num.gameObject.SetActive(false); }

        // Remove unused points
        int indexOfEnd = points.IndexOf(objToRemove);
        for (int i=points.Count-1; i>=indexOfEnd; i--)
        {
            GameObject toRemove = points[i];
            points.RemoveAt(i);
            Destroy(toRemove);
        }

        // Get translation points relative to world
        relativePointPositions = new Vector3[points.Count];
        absolutePointPositions = new Vector3[points.Count];
        for (int i=0; i<relativePointPositions.Length; i++)
        {
            relativePointPositions[i] = (i==0) ? Vector3.zero : points[i].transform.localPosition;
            absolutePointPositions[i] = points[i].transform.position;
        }

        // Play sample animation
        string printout = "Points: ";
        foreach (Vector3 vec in relativePointPositions) { printout += vec.ToString() + ", "; }
        print(printout);
        Translate();

        // activate collider
        SphereCollider[] colliders = GetComponentsInChildren<SphereCollider>(true);
        foreach (SphereCollider collider in colliders)
        {
            if (collider.gameObject.CompareTag("SKETCH_ProximityTranslation")) {
                collider.gameObject.SetActive(true);
                break;
            }
        }
        GetComponent<HandDraggable>().StoppedDragging += UpdateVarsOnSketchDragged;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().
        GetComponent<HandDraggable>().StartedDragging += SketchDragStarted;
        //SetupComplete.Invoke();
    }

    #region Helper

    /// <summary>
    /// Creates a child GameObject used to help user create a path
    /// Path is what the Sketch will translate along (really just points)
    /// </summary>
    void generatePoint()
    {
        GameObject point = Instantiate(Template, transform);
        point.SetActive(true);
        points.Add(point);
        point.GetComponent<TextMesh>().text = "0";
        point.GetComponent<TextMesh>().fontSize = 14;
        point.GetComponent<TranslationPointManager>().index = points.Count -1;


        /** Comment on reverse scale:
         * Child's scale is a function of parent's scale AND child's relative rotation.
         * Scale acts in x, y, and z directions
         * So when object is rotated, it's parts in the x, y, and z directions change.
         * And so, it's scale needs to change with respect to that rotation.
         * Currently don't have the equation to do this.
         */
        // Counteract Scale
        /*Vector3 sketchScale = transform.localScale;
        Vector3 pointScale = point.transform.localScale;
        Vector3 pointWorldScale = point.transform.lossyScale;
        print(string.Format("Sketch Scale: {0}; Point Local Scale: {1}; Point World Scale: {2}", sketchScale, pointScale, pointWorldScale));

        // Reverse Scale of Parent
        pointScale.x /= sketchScale.x * 25;
        pointScale.y /= sketchScale.z * 25; // yes, Z
        pointScale.z /= sketchScale.z * 25;
        point.transform.localScale = pointScale;
        print(string.Format("New Scale for point: {0}", pointScale));*/
    }

    /// <summary>
    /// Moves the Sketch in the direction of the next point
    /// If the sketch gets within range of that point,
    /// It starts moving toward the next point.
    /// If it reaches the end, it goes back to the start
    /// </summary>
    public void Translate()
    {
        moving = true;

        for (int i=0; i<absolutePointPositions.Length; i++)
        {
            // When Sketch hits a point set target to the next point
            if (Vector3.Distance(transform.position, absolutePointPositions[i]) < 0.01f)
            {
                if (i == absolutePointPositions.Length - 1)
                {
                    moving = false;
                    // Should add pause before moving back? Wait until person looks away?
                    transform.position = absolutePointPositions[0]; //back to origin
                    target = absolutePointPositions[1];
                    return;
                }
                else
                {
                    target = absolutePointPositions[i + 1];
                    break;
                }
            }
        }
        //float time = Vector3.Distance(transform.position, relativePointPositions[0]) / translationSpeed;
        Vector3 deltaX = target - transform.position;
        Vector3 deltaX_unit = Vector3.Normalize(deltaX) * Time.deltaTime * translationSpeed;


        transform.Translate(deltaX_unit, Space.World);
    }

    /// <summary>
    /// Draws line between 2 objects
    /// </summary>
    /// <param name="pt1">The first GameObject</param>
    /// <param name="pt2">The second GameObject</param>
    private void drawLineBetween(GameObject pt1, GameObject pt2)
    {
        GameObject baseLineObject = new GameObject("LineObject");
        GameObject lineObject = Instantiate(baseLineObject, transform);
        Destroy(baseLineObject);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        Vector3[] pointPositions = new Vector3[2];
        pointPositions[0] = pt1.transform.position;
        pointPositions[1] = pt2.transform.position;
        line.SetPositions(pointPositions);

        // line formatting
        line.startWidth = 0.003f;
        line.endWidth = 0.003f;
        line.startColor = Color.blue;
        line.endColor = Color.blue;
    }
    #endregion Helper
    #endregion Methods

    public TranslationData ExportData()
    {
        TranslationData data = new TranslationData();
        data.translationSpeed = this.translationSpeed;
        data.points = this.points;
        data.relativePointPositions = this.relativePointPositions;
        data.absolutePointPositions = this.absolutePointPositions;
        data.initialRenderState = this.initialRenderState;

        return data;
    }
    public void ImportData(TranslationData data)
    {
        //this.translationSpeed = data.translationSpeed;
        this.points = data.points;
        this.relativePointPositions = data.relativePointPositions;
        this.absolutePointPositions = data.absolutePointPositions;
        this.initialRenderState = data.initialRenderState;


        SetupCompleted = true;
        LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();

        // Hide all elements
        lines = GetComponentsInChildren<LineRenderer>();
        TextMesh[] numbers = GetComponentsInChildren<TextMesh>();
        foreach (LineRenderer line in lines) { line.gameObject.SetActive(false); }
        foreach (TextMesh num in numbers) { num.gameObject.SetActive(false); }

        // Play sample animation
        string printout = "Points: ";
        foreach (Vector3 vec in relativePointPositions) { printout += vec.ToString() + ", "; }
        print(printout);
        Translate();

        // activate collider
        SphereCollider[] colliders = GetComponentsInChildren<SphereCollider>(true);
        foreach (SphereCollider collider in colliders)
        {
            if (collider.gameObject.CompareTag("SKETCH_ProximityTranslation"))
            {
                collider.gameObject.SetActive(true);
                break;
            }
        }
        GetComponent<HandDraggable>().StoppedDragging += UpdateVarsOnSketchDragged;
        //GetComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>().
        GetComponent<HandDraggable>().StartedDragging += SketchDragStarted;
    }
}

[System.Serializable]
public class TranslationData
{
    public float translationSpeed;
    public List<GameObject> points = new List<GameObject>();
    public Vector3[] relativePointPositions;
    public Vector3[] absolutePointPositions;
    public bool initialRenderState;
}


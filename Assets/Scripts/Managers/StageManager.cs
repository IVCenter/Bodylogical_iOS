using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This manager controls the stage itself (transform, etc),
/// as well as the transitions between different states.
/// </summary>
public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }

    /// <summary>
    /// Parent object for the stage.
    /// </summary>
    public GameObject stage;

    public Transform stageCenter;
    [SerializeField] private GameObject stageObject;
    [SerializeField] private GameObject header;

    // Visualization transition
    [SerializeField] private Transform plane;
    [SerializeField] private float moveTime = 2f;
    [SerializeField] private float waitTime = 1f;

    [HideInInspector] public Visualization currVis;
    private Dictionary<Visualization, GameObject> visDict;
    [HideInInspector] public bool stageReady;

    // Shader property hashes
    private static readonly int renderBack = Shader.PropertyToID("_RenderBack");
    private static readonly int planeNormal = Shader.PropertyToID("_PlaneNormal");
    private static readonly int planePosition = Shader.PropertyToID("_PlanePosition");

    /// <summary>
    /// Used for control panel initialization
    /// </summary>
    private bool initialized;

    /// <summary>
    /// Layer mask for AR planes
    /// </summary>
    private static readonly int planeLayerMask = 1 << 10;

    #region Unity routines

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        ToggleStage(false);

        visDict = new Dictionary<Visualization, GameObject> {
            {Visualization.Activity, ActivityManager.Instance.activityParent},
            {Visualization.LineChart, LineChartManager.Instance.yearPanelParent},
            {Visualization.Prius, PriusManager.Instance.priusParent}
        };
    }

    #endregion

    #region Stage control

    /// <summary>
    /// This will be called in image tracking mode. Copies the transform of the image to the stage.
    /// </summary>
    public void CopyTransform() {
        Transform planeTransform = PlaneManager.Instance.Planes[0].transform;
        stage.transform.position = planeTransform.position;
        stage.transform.rotation = planeTransform.rotation;
    }

    /// <summary>
    /// This will be called in plane tracking mode. Moves the stage on the plane.
    /// </summary>
    public void UpdateStageTransform() {
        stageObject.SetActive(true);

        // We want the stage to stay on the plane. Get the center point of the screen,
        // make a raycast to see if the center point projects to the plane.
        // If so, relocate the stage.
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.CenterPos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, planeLayerMask)) {
            Vector3 centerPos = hit.point;
            stage.transform.position += centerPos - stageCenter.position;
            // Align stage rotation to the camera rotation
            Vector3 rotation = Vector3.zero;
            rotation.y = Camera.main.transform.eulerAngles.y;
            stage.transform.eulerAngles = rotation;
        }
    }

    public void ToggleStage(bool enable) {
        stage.SetActive(enable);
    }

    public void HideStageObject() {
        stageObject.SetActive(false);
    }

    public void Reset() {
        stageReady = false;
        ToggleStage(false);
    }

    #endregion

    #region Switching Visualization

    /// <summary>
    /// When the button is pressed, switch to line chart visualization.
    /// </summary>
    public void SwitchLineChart() {
        AppStateManager.Instance.CurrState = AppState.VisLineChart;

        header.SetActive(false);
        ActivityManager.Instance.ToggleActivity(false);
        LineChartManager.Instance.ToggleLineChart(true);
        StartCoroutine(LineChartManager.Instance.StartLineChart(visDict[currVis]));
        currVis = Visualization.LineChart;
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchActivity() {
        AppStateManager.Instance.CurrState = AppState.VisActivity;

        if (!initialized) {
            initialized = true;
            ControlPanelManager.Instance.ToggleInterventions(true);
            ControlPanelManager.Instance.ToggleAnimations(true);
            ControlPanelManager.Instance.TogglePrev(true);
            ControlPanelManager.Instance.ToggleHandle(true);
        }

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        LineChartManager.Instance.ToggleLineChart(false);
        ActivityManager.Instance.ToggleActivity(true);
        StartCoroutine(ActivityManager.Instance.StartActivity(visDict[currVis]));
        currVis = Visualization.Activity;
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        AppStateManager.Instance.CurrState = AppState.VisPrius;

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        LineChartManager.Instance.ToggleLineChart(false);
        ActivityManager.Instance.ToggleActivity(false);
        StartCoroutine(PriusManager.Instance.StartPrius(visDict[currVis]));
        currVis = Visualization.Prius;
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetVisualizations() {
        header.SetActive(false);
        ActivityManager.Instance.ToggleActivity(false);
        LineChartManager.Instance.ToggleLineChart(false);
        initialized = false;
    }

    #endregion

    #region Visualization Transitions

    /// <summary>
    /// Performs a smooth transition animation between two visualizations.
    /// Notice that this only operates on two visualization objects, and does not
    /// manage other things such as year header.
    /// The reason to have vis1 explicitly passed in instead of having it set to
    /// visDict[currentVis] is because the time point when currentVis is accessed
    /// is unknown (this is an IEnumerator) and it might be modified by the time
    /// of access.
    /// </summary>
    /// <returns>The visualization.</returns>
    /// <param name="vis1">Visualization module to be hidden.</param>
    /// <param name="vis2">Visualization module to be shown.</param>
    /// <param name="callback">Optional callback function to be executed after the transition.</param>
    public IEnumerator ChangeVisualization(GameObject vis1, GameObject vis2) {
        plane.gameObject.SetActive(true);
        int moveTimeStep = (int) (moveTime / Time.deltaTime);
        float moveTransStep = plane.localPosition.y * 1.05f / moveTimeStep;
        Vector3 movement = new Vector3(0, -moveTransStep, 0);

        // Hide tutorials
        TutorialManager.Instance.ClearTutorial();

        // Initialize archetype for clipping
        Material archetypeMat = ArchetypeManager.Instance.Selected.Mat;
        archetypeMat.SetInt(renderBack, 1);

        // Now, find out all objects that can be clipped by the plane, and all that cannot.
        // For normal objects, find if "PlaneNormal" is in the material properties.
        // Unclippable objects and canvases will be hidden at the start of the animation,
        // and will be displayed after the animation is complete.
        List<GameObject> unclippables = new List<GameObject>();
        List<Material> vis1Clippables = new List<Material>();
        List<Material> vis2Clippables = new List<Material>();

        List<Renderer> vis1Renderers = vis1.transform.SearchAllWithType<Renderer>();
        List<Canvas> vis1Canvases = vis1.transform.SearchAllWithType<Canvas>();

        foreach (Renderer r in vis1Renderers) {
            bool clippable = false;
            foreach (Material m in r.materials) {
                if (m.HasProperty(planeNormal)) {
                    vis1Clippables.Add(m);
                    clippable = true;
                }
            }
            
            if (!clippable && r.gameObject.activeSelf) {
                unclippables.Add(r.gameObject);
            }
        }

        foreach (Canvas c in vis1Canvases) {
            if (c.gameObject.activeSelf) {
                unclippables.Add(c.gameObject);
            }
        }

        List<Renderer> vis2Renderers = vis2.transform.SearchAllWithType<Renderer>();
        List<Canvas> vis2Canvases = vis2.transform.SearchAllWithType<Canvas>();

        foreach (Renderer r in vis2Renderers) {
            bool clippable = false;
            foreach (Material m in r.materials) {
                if (m.HasProperty(planeNormal)) {
                    vis2Clippables.Add(m);
                    clippable = true;
                }
            }
             
            if (!clippable && r.gameObject.activeSelf) {
                unclippables.Add(r.gameObject);
            }
        }

        foreach (Canvas c in vis2Canvases) {
            if (c.gameObject.activeSelf) {
                unclippables.Add(c.gameObject);
            }
        }


        // Hide all unclippables
        foreach (GameObject g in unclippables) {
            g.SetActive(false);
        }

        // Set render back to all clippables
        foreach (Material m in vis1Clippables) {
            if (m.HasProperty(renderBack)) {
                m.SetInt(renderBack, 1);
            }
        }

        foreach (Material m in vis2Clippables) {
            if (m.HasProperty(renderBack)) {
                m.SetInt(renderBack, 1);
            }
        }

        // Plane goes down
        for (int i = 0; i < moveTimeStep; i++) {
            plane.Translate(movement);
            archetypeMat.SetVector(planePosition, plane.position);
            foreach (Material m in vis1Clippables) {
                m.SetVector(planePosition, plane.position);
            }

            yield return null;
        }

        vis1.SetActive(false);

        yield return new WaitForSeconds(waitTime);

        // Plane goes up
        movement.y = moveTransStep; // Reverse movement y
        vis2.SetActive(true);
        for (int i = 0; i < moveTimeStep; i++) {
            plane.Translate(movement);
            archetypeMat.SetVector(planePosition, plane.position);
            foreach (Material m in vis2Clippables) {
                m.SetVector(planePosition, plane.position);
            }

            yield return null;
        }

        archetypeMat.SetInt(renderBack, 0);
        // Show all unclippables
        foreach (GameObject g in unclippables) {
            g.SetActive(true);
        }

        // Set render back to all clippables
        foreach (Material m in vis1Clippables) {
            if (m.HasProperty(renderBack)) {
                m.SetInt(renderBack, 0);
            }
        }

        foreach (Material m in vis2Clippables) {
            if (m.HasProperty(renderBack)) {
                m.SetInt(renderBack, 0);
            }
        }

        plane.gameObject.SetActive(false);
        yield return null;
    }

    #endregion
}
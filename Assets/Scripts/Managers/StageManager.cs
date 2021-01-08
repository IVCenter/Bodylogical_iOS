using System.Collections;
using UnityEngine;

/// <summary>
/// This manager controls the stage itself (transform, etc).
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

    [SerializeField] private GameObject mountain;
    [SerializeField] private Transform mountainTop;
    [SerializeField] private GameObject sidewalk;
    public BackwardsProps[] props;
    
    public bool StageReady { get; set; }

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

    public void ToggleStage(bool on) {
        stage.SetActive(on);
    }

    public void HideStageObject() {
        stageObject.SetActive(false);
    }

    public void ResetStage() {
        StageReady = false;
        ToggleStage(false);
    }

    #endregion

    #region Switching Visualization

    /// <summary>
    /// Display the "top of the hill" and the sidewalks, the display model will walk to the top of the hill, and
    /// the three performers will appear. Then, initialize the activity visualization for the three performers.
    /// </summary>
    public void StartVisualizations() {
        ArchetypeManager.Instance.Selected.Icon.SetActive(false);
        StartCoroutine(Transition());
    }

    private IEnumerator Transition() {
        mountain.SetActive(true);
        yield return ArchetypeManager.Instance.MoveSelectedTo(mountainTop.position);
        sidewalk.SetActive(true);
        ArchetypeManager.Instance.PerformerParent.gameObject.SetActive(true);
        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.Performers.Values) {
            performer.Activity.Toggle(true);
        }
    }

    /// <summary>
    /// When the button is pressed, switch to line chart visualization.
    /// </summary>
    public void SwitchLineChart() {
        //AppStateManager.Instance.CurrState = AppState.VisLineChart;

        header.SetActive(false);
        //ActivityManager.Instance.ToggleActivity(false);
        // LineChartManager.Instance.ToggleLineChart(true);
        // LineChartManager.Instance.StartLineChart();
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchActivity() {
        //AppStateManager.Instance.CurrState = AppState.VisActivity;

        if (!initialized) {
            initialized = true;
            ControlPanelManager.Instance.ToggleInterventions(true);
            ControlPanelManager.Instance.ToggleAnimations(true);
            ControlPanelManager.Instance.TogglePrev(true);
            ControlPanelManager.Instance.ToggleHandle(true);
        }

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        //LineChartManager.Instance.ToggleLineChart(false);
        //ActivityManager.Instance.ToggleActivity(true);
        //ActivityManager.Instance.StartActivity();
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        //AppStateManager.Instance.CurrState = AppState.VisPrius;

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        //LineChartManager.Instance.ToggleLineChart(false);
        //ActivityManager.Instance.ToggleActivity(false);
        //PriusManager.Instance.StartPrius();
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetVisualizations() {
        header.SetActive(false);
        //ActivityManager.Instance.ToggleActivity(false);
        //LineChartManager.Instance.ToggleLineChart(false);
        initialized = false;
    }

    #endregion
}
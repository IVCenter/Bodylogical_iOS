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
    [SerializeField] private DisplayInternals displayInternals;
    public BackwardsProps[] props;

    [SerializeField] private Transform tutorialTransform;

    /// <summary>
    /// When the user enters the visualizations, the avatar will walk to a "podium", which would then rise from the
    /// ground along with the street scene. In editor mode, the y-coordinate for the ground is simply 0; however, in AR
    /// space, the y-coordinate depends on the device's startup position and will change each time. Thus, after the user
    /// confirms the stage, we need to sync the y-coordinate to the shaders.
    /// </summary>
    [SerializeField] private MeshRenderer[] riseFromGroundObjects;

    private static readonly int PlaneID = Shader.PropertyToID("_PlanePosition");

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
        displayInternals.Initialize(); // Only executes once
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

    public void SyncYCoord() {
        foreach (MeshRenderer renderer in riseFromGroundObjects) {
            renderer.material.SetVector(PlaneID, new Vector4(0, stageObject.transform.position.y, 0, 0));
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
        ArchetypeManager.Instance.displayer.icon.SetActive(false);
        StartCoroutine(Transition());
        AppStateManager.Instance.CurrState = AppState.Visualizations;
    }

    private IEnumerator Transition() {
        ArchetypeManager.Instance.displayer.panel.ToggleText(false);
        TimeProgressManager.Instance.Cycle(false);
        mountain.SetActive(true);
        yield return ArchetypeManager.Instance.displayer.MoveTo(mountainTop.position);

        // Before we enable all performers, we need to ensure that the health data is present for all of them.
        if (!ArchetypeManager.Instance.DataReady) {
            TutorialManager.Instance.ShowInstruction("Instructions.CalculateData");
            yield return new WaitUntil(() => ArchetypeManager.Instance.DataReady);
            TutorialManager.Instance.ClearInstruction();
        }

        // Switch to the first visualization: Activity
        sidewalk.SetActive(true);
        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            performer.CurrentVisualization = Visualization.Activity;
            performer.gameObject.SetActive(true);
            performer.stats.BuildStats();
            performer.panel.SetColor();
            StartCoroutine(performer.activity.Toggle(true));
        }

        displayInternals.gameObject.SetActive(true);

        EnableTimeline();

        // Switch from input panel to control panel
        ControlPanelManager.Instance.ToggleDataPanel(false);
        ControlPanelManager.Instance.ToggleControlPanel(true);
    }

    /// <summary>
    /// Enable the controls for timelines.
    /// </summary>
    private void EnableTimeline() {
        ControlPanelManager.Instance.ToggleHandle(true);

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText(0);
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetVisualizations() {
        ArchetypeManager.Instance.displayer.panel.ToggleText(false);
        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            performer.CurrentVisualization = Visualization.None;
        }

        header.SetActive(false);
        mountain.SetActive(false);
        sidewalk.SetActive(false);
        foreach (BackwardsProps prop in props) {
            prop.ResetProps();
        }
    }

    #endregion

    #region Tutorials

    public void ActivityTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.ActivityTitle", "Tutorials.ActivityText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform, () => TimeProgressManager.Instance.Playing,
            postCallback: TimeProgressManager.Instance.ShowTut1);
    }

    public void PriusTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.PriusTitle", "Tutorials.PriusText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform, () => {
                foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
                    if (performer.CurrentVisualization == Visualization.Stats) {
                        return true;
                    }
                }

                return false;
            },
            postCallback: StatsTutorial);
    }

    private void StatsTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.StatsTitle", "Tutorials.StatsText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform);
    }

    #endregion
}
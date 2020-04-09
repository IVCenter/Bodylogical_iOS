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
    public GameObject header;

    // Visualization transition
    [SerializeField] private GameObject stageBox;
    [SerializeField] private Transform leftDoor, rightDoor;
    [SerializeField] private float doorTime = 1.0f;
    [SerializeField] private float moveTime = 1.0f;

    [HideInInspector] public Visualization currVis;
    public Dictionary<Visualization, GameObject> visDict;
    [HideInInspector] public bool stageReady;

    #region Unity routines
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        ToggleStage(false);

        visDict = new Dictionary<Visualization, GameObject> {
            { Visualization.Activity, ActivityManager.Instance.activityParent },
            { Visualization.LineChart, LineChartManager.Instance.yearPanelParent },
            { Visualization.Prius, PriusManager.Instance.priusParent }
        };
    }
    #endregion

    #region Stage control
    public void UpdateStageTransform() {
        stageObject.SetActive(true);

        // We want the stage to stay on the plane. Get the center point of the screen,
        // make a raycast to see if the center point projects to the plane.
        // If so, relocate the stage.
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.CenterPos);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.collider.GetComponent<PlaneInteract>() != null) {
                Vector3 centerPos = hit.point;
                Vector3 diff = stage.transform.position - stageCenter.position;
                stage.transform.position = centerPos + diff;
                AdjustStageRotation(PlaneManager.Instance.MainPlane);
            }
        }
    }

    public void ToggleStage(bool enable) {
        stage.SetActive(enable);
    }

    public void HideStageObject() {
        stageObject.SetActive(false);
    }

    private void AdjustStageRotation(GameObject plane) {
        stage.transform.rotation = plane.transform.rotation;
        // Rotate such that the front faces the camera.
        while (Vector3.Dot(stage.transform.position - Camera.main.transform.position, stage.transform.forward) < 0) {
            stage.transform.Rotate(0, 90, 0);
        }
    }

    public void Reset() {
        stageReady = false;
        ToggleStage(false);
    }
    #endregion

    #region Switching Visualization
    private bool lcTutShown, actTutShown, priTutShown;

    /// <summary>
    /// When the button is pressed, switch to line chart visualization.
    /// </summary>
    public void SwitchLineChart() {
        AppStateManager.Instance.currState = AppState.VisLineChart;

        if (!lcTutShown) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam text1 = new TutorialParam("Tutorials.LCIntroTitle", "Tutorials.LCIntroText1");
            TutorialParam text2 = new TutorialParam("Tutorials.LCIntroTitle", "Tutorials.LCIntroText2");
            TutorialManager.Instance.ShowTutorial(new TutorialParam[] { text1, text2 }, new Vector3(0.8f, 0.25f, 0f));
            lcTutShown = true;
        }

        header.SetActive(false);
        ActivityManager.Instance.ToggleActivity(false);
        PriusManager.Instance.TogglePrius(false);
        LineChartManager.Instance.ToggleLineChart(true);
        StartCoroutine(LineChartManager.Instance.StartLineChart(visDict[currVis]));
        currVis = Visualization.LineChart;
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchActivity() {
        AppStateManager.Instance.currState = AppState.VisActivity;

        if (!actTutShown) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam text = new TutorialParam("Tutorials.ActIntroTitle", "Tutorials.ActIntroText");
            TutorialManager.Instance.ShowTutorial(text, new Vector3(0.8f, 0.25f, 0f));
            actTutShown = true;
        }

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        LineChartManager.Instance.ToggleLineChart(false);
        PriusManager.Instance.TogglePrius(false);
        ActivityManager.Instance.ToggleActivity(true);
        StartCoroutine(ActivityManager.Instance.StartActivity(visDict[currVis]));
        currVis = Visualization.Activity;
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        AppStateManager.Instance.currState = AppState.VisPrius;

        if (!priTutShown) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam text = new TutorialParam("Tutorials.PriIntroTitle", "Tutorials.PriIntroText");
            TutorialManager.Instance.ShowTutorial(text, new Vector3(0.8f, 0.25f, 0f));
            priTutShown = true;
        }

        header.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        LineChartManager.Instance.ToggleLineChart(false);
        ActivityManager.Instance.ToggleActivity(false);
        PriusManager.Instance.TogglePrius(true);
        StartCoroutine(PriusManager.Instance.StartPrius(visDict[currVis]));
        currVis = Visualization.Prius;
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetVisualizations() {
        header.SetActive(false);
        ActivityManager.Instance.ToggleActivity(false);
        PriusManager.Instance.TogglePrius(false);
        LineChartManager.Instance.ToggleLineChart(false);
        // ToggleLineChart will enable line chart button.
        // However, during MasterManager's Reset() a call will be made to ButtonSequenceManager
        // thus automatically resetting all buttons. So no need to worry.
    }

    public void ResetTutorial() {
        lcTutShown = false;
        actTutShown = false;
        priTutShown = false;
    }
    #endregion

    #region Visualization Transitions
    /// <summary>
    /// Performs a smooth transition animation between two visualizations.
    /// See: https://www.youtube.com/watch?v=xgakdcEzVwg&feature=youtu.be&t=151
    /// Notice that this only operates on two visualization objects, and does not
    /// manage other things such as year header.
    /// The reason to have vis1 explicitly passed in instead of having it set to
    /// visDict[currentVis] is because the time point when currentVis is accessed
    /// is unknown (this is an IEnumerator) and it might be modified by the time
    /// of access.
    /// </summary>
    /// <returns>The visualization.</returns>
    /// <param name="vis1">Visualization object to be hidden.</param>
    /// <param name="vis2">Visualization object to be shown.</param>
    /// <param name="hideChar">If the archetype needs to be shown or hidden.</param>
    public IEnumerator ChangeVisualization(GameObject vis1, GameObject vis2, bool hideChar = false) {
        stageBox.SetActive(true);

        int doorTimeStep = (int)(doorTime / Time.deltaTime);
        float doorTransStep = 1.0f / doorTimeStep;

        // Open door: Shift localPosition.x. left: 0 -> -1, right: 0 -> 1
        // Translate takes two parameters; the second defaults to Space.Self.
        for (int i = 0; i < doorTimeStep; i++) {
            leftDoor.Translate(new Vector3(-doorTransStep, 0, 0));
            rightDoor.Translate(new Vector3(doorTransStep, 0, 0));
            yield return null;
        }

        int moveTimeStep = (int)(moveTime / Time.deltaTime);
        float moveTransStep = 1.0f / moveTimeStep;

        // vis1 (and archetype) goes down
        for (int i = 0; i < moveTimeStep; i++) {
            vis1.transform.Translate(new Vector3(0, -moveTransStep, 0));
            ArchetypeManager.Instance.SelectedModel.transform.Translate(new Vector3(0, -moveTransStep, 0));
            yield return null;
        }
        vis1.SetActive(false);
        vis1.transform.localPosition = new Vector3(0, 0, 0);

        if (hideChar) {
            ArchetypeManager.Instance.SelectedModel.SetActive(false);
        } else {
            ArchetypeManager.Instance.SelectedModel.SetActive(true);
        }

        // vis2 (and archetype) goes up
        vis2.transform.localPosition = new Vector3(0, -1.0f, 0);
        vis2.SetActive(true);
        for (int i = 0; i < moveTimeStep; i++) {
            vis2.transform.Translate(new Vector3(0, moveTransStep, 0));
            ArchetypeManager.Instance.SelectedModel.transform.Translate(new Vector3(0, moveTransStep, 0));
            yield return null;
        }

        // Close door: Shift localPosition.x. left: -1 -> 0, right: 1 -> 0
        for (int i = 0; i < doorTimeStep; i++) {
            leftDoor.Translate(new Vector3(doorTransStep, 0, 0));
            rightDoor.Translate(new Vector3(-doorTransStep, 0, 0));
            yield return null;
        }

        stageBox.SetActive(false);
    }
    #endregion
}
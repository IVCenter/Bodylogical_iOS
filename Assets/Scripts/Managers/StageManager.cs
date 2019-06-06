using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }

    public GameObject stage;
    public GameObject stageObject;
    public Transform[] positionList;
    public GameObject yearHeader;
    public Transform characterParent;

    // Visualization transition
    public GameObject stageBox;
    public Transform leftDoor, rightDoor;
    public float doorTime = 1.0f;
    public float moveTime = 1.0f;

    private Dictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private bool isAnimating;

    public Transform CenterTransform { get { return stage.transform.GetChild(0); } }

    [HideInInspector]
    public Visualization currVis = Visualization.LineChart;
    public Dictionary<Visualization, GameObject> visDict;

    #region Unity routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    void Start() {
        posAvailableMap = new Dictionary<Transform, bool>();

        foreach (Transform trans in positionList) {
            posAvailableMap.Add(trans, true);
        }

        futureBlue = stageObject.GetComponent<MeshRenderer>().material.color;
        colorWhite = new Color(0, 1, 1, 0.42f);

        DisableStage();

        visDict = new Dictionary<Visualization, GameObject> {
            { Visualization.Activity, ActivityManager.Instance.activityParent },
            { Visualization.LineChart, YearPanelManager.Instance.yearPanelParent },
            { Visualization.Prius, PriusManager.Instance.priusParent }
        };
    }
    #endregion

    #region Stage control
    /// <summary>
    /// For each of the archetypes, create a model.
    /// </summary>
    public void BuildStage() {
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            human.CreateModel();
        }
    }

    public Transform GetAvailablePosInWorld() {
        foreach (Transform trans in positionList) {
            if (posAvailableMap[trans]) {
                posAvailableMap[trans] = false;
                return trans;
            }
        }
        return null;
    }

    public void UpdateStageTransform() {
        if (!stage.activeSelf) {
            stage.SetActive(true);
        }

        if (!stageObject.GetComponent<MeshRenderer>().enabled) {
            stageObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (InputManager.Instance.cursor.FocusedObj != null) {
            GameObject obj = InputManager.Instance.cursor.FocusedObj;

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>() != null) {
                Vector3 cursorPos = InputManager.Instance.cursor.CursorPosition;
                Vector3 diff = stage.transform.position - CenterTransform.position;
                stage.transform.position = cursorPos + diff;
                AdjustStageRotation(PlaneManager.Instance.MainPlane);
            }
        }

        Color lerpedColor = Color.Lerp(colorWhite, futureBlue, Mathf.PingPong(Time.time, 1));

        stageObject.GetComponent<MeshRenderer>().material.color = lerpedColor;
    }

    public void DisableStage() {
        stage.SetActive(false);
    }

    public void SettleStage() {
        stageObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void AdjustStageRotation(GameObject plane) {
        stage.transform.rotation = plane.transform.rotation;

        while (Vector3.Dot((stage.transform.position - Camera.main.transform.position), stage.transform.forward) < 0) {
            stage.transform.Rotate(0, 90, 0);
        }
    }

    /// <summary>
    /// Called when stage is settled. Loop among different poses.
    /// </summary>
    public void SetHumanIdlePose() {
        foreach (Transform trans in characterParent) {
            trans.Find("model").GetChild(0).GetComponent<Animator>().SetTrigger("IdlePose");
        }
    }
    #endregion

    #region Visualizations Switching
    private bool lcTutShown, actTutShown, priTutShown;

    /// <summary>
    /// When the button is pressed, switch to line chart visualization.
    /// </summary>
    public void SwitchLineChart() {
        MasterManager.Instance.currPhase = GamePhase.VisLineChart;

        if (!lcTutShown) {
            TutorialParam text1 = new TutorialParam("Tutorials.LCIntroTitle", "Tutorials.LCIntroText1");
            TutorialParam text2 = new TutorialParam("Tutorials.LCIntroTitle", "Tutorials.LCIntroText2");
            TutorialManager.Instance.ShowTutorial(new TutorialParam[] { text1, text2 });
            lcTutShown = true;
        }

        yearHeader.SetActive(false);
        ActivityManager.Instance.ToggleActivity(false);
        PriusManager.Instance.TogglePrius(false);
        YearPanelManager.Instance.ToggleLineChart(true);
        StartCoroutine(YearPanelManager.Instance.StartLineChart(visDict[currVis]));
        currVis = Visualization.LineChart;
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchActivity() {
        MasterManager.Instance.currPhase = GamePhase.VisActivity;

        if (!actTutShown) {
            TutorialParam text = new TutorialParam("Tutorials.ActIntroTitle", "Tutorials.ActIntroText");
            TutorialManager.Instance.ShowTutorial(text);
            actTutShown = true;
        }

        yearHeader.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        YearPanelManager.Instance.ToggleLineChart(false);
        PriusManager.Instance.TogglePrius(false);
        ActivityManager.Instance.ToggleActivity(true);
        StartCoroutine(ActivityManager.Instance.StartActivity(visDict[currVis]));
        currVis = Visualization.Activity;
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        MasterManager.Instance.currPhase = GamePhase.VisPrius;

        if (!priTutShown) {
            TutorialParam text = new TutorialParam("Tutorials.PriIntroTitle", "Tutorials.PriIntroText");
            TutorialManager.Instance.ShowTutorial(text);
            priTutShown = true;
        }

        yearHeader.SetActive(true);
        TimeProgressManager.Instance.UpdateHeaderText();

        YearPanelManager.Instance.ToggleLineChart(false);
        ActivityManager.Instance.ToggleActivity(false);
        PriusManager.Instance.TogglePrius(true);
        StartCoroutine(PriusManager.Instance.StartPrius(visDict[currVis]));
        currVis = Visualization.Prius;
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetVisualizations() {
        yearHeader.SetActive(false);
        ActivityManager.Instance.ToggleActivity(false);
        PriusManager.Instance.TogglePrius(false);
        YearPanelManager.Instance.ToggleLineChart(false);
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
            HumanManager.Instance.SelectedHuman.transform.Translate(new Vector3(0, -moveTransStep, 0));
            yield return null;
        }
        vis1.SetActive(false);
        vis1.transform.localPosition = new Vector3(0, 0, 0);

        if (hideChar) {
            HumanManager.Instance.SelectedHuman.SetActive(false);
        } else {
            HumanManager.Instance.SelectedHuman.SetActive(true);
        }

        // vis2 (and archetype) goes up
        vis2.transform.localPosition = new Vector3(0, -1.0f, 0);
        vis2.SetActive(true);
        for (int i = 0; i < moveTimeStep; i++) {
            vis2.transform.Translate(new Vector3(0, moveTransStep, 0));
            HumanManager.Instance.SelectedHuman.transform.Translate(new Vector3(0, moveTransStep, 0));
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
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Collections.Hybrid.Generic;


public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }

    public GameObject stage;
    public GameObject stageObject;
    public GameObject controlPanel;
    public Transform[] positionList;
    public PlayPauseButton playPauseButton;
    public GameObject yearHeader;
    public GameObject timeSlider;

    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private Vector3 controlPanelInitLocPos;
    private bool isAnimating;

    private bool isTimePlaying;
    private IEnumerator timeProgressCoroutine;

    public Transform CenterTransform { get { return stage.transform.GetChild(0); } }
    private Text HeaderText { get { return yearHeader.transform.GetChild(0).GetComponent<Text>(); } }
    private SliderInteract Interact { get { return timeSlider.transform.GetChild(0).GetComponent<SliderInteract>(); } }
    private Text SliderText { get { return timeSlider.transform.GetChild(2).GetChild(0).GetComponent<Text>(); } }

    public enum VisualizationType {
        Animation, Prius, LineChart
    };
    public VisualizationType Visualization { get; private set; }

    public string Path { get; private set; }
    public int Year { get; private set; }

    #region Unity routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        controlPanelInitLocPos = new Vector3(controlPanel.transform.localPosition.x, controlPanel.transform.localPosition.y, controlPanel.transform.localPosition.z);
        isAnimating = false;
    }


    void Start() {
        posAvailableMap = new LinkedListDictionary<Transform, bool>();

        foreach (Transform trans in positionList) {
            posAvailableMap.Add(trans, true);
        }

        futureBlue = stageObject.GetComponent<MeshRenderer>().material.color;
        colorWhite = new Color(0, 1, 1, 0.42f);

        DisableControlPanel();
        DisableStage();
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

    /// <summary>
    /// When the stage is settled and the archetype is chosen, show the control panel.
    /// </summary>
    public void EnableControlPanel() {
        if (!isAnimating) {
            StartCoroutine(FadeUpCP());
        }
    }

    /// <summary>
    /// An animation to bring the control panel to the stage.
    /// </summary>
    IEnumerator FadeUpCP() {
        controlPanel.SetActive(true);

        float animation_time = 2.5f;
        float time_passed = 0;
        isAnimating = true;

        controlPanel.transform.localPosition = new Vector3(controlPanel.transform.localPosition.x, -10f, controlPanel.transform.localPosition.z);

        while (time_passed < animation_time) {
            controlPanel.transform.localPosition = Vector3.Lerp(controlPanel.transform.localPosition, controlPanelInitLocPos, 0.03f);

            time_passed += Time.deltaTime;
            yield return null;
        }

        controlPanel.transform.localPosition = controlPanelInitLocPos;

        isAnimating = false;

        yield return null;
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

        if (CursorManager.Instance.cursor.FocusedObj != null) {
            GameObject obj = CursorManager.Instance.cursor.FocusedObj;

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>() != null) {
                Vector3 cursorPos = CursorManager.Instance.cursor.CursorPosition;
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

    public void DisableControlPanel() {
        controlPanel.SetActive(false);
    }

    private void AdjustStageRotation(GameObject plane) {
        stage.transform.rotation = plane.transform.rotation;

        while (Vector3.Dot((stage.transform.position - Camera.main.transform.position), stage.transform.forward) < 0) {
            stage.transform.Rotate(0, 90, 0);
        }
    }
    #endregion

    #region Visualizations
    /// <summary>
    /// When the button is pressed, switch to line chart visualization.
    /// </summary>
    public void SwitchLineChart() {
        Visualization = VisualizationType.LineChart;

        yearHeader.SetActive(false);
        AnimationManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(false);
        YearPanelManager.Instance.ToggleLineChart(true);

        StartCoroutine(YearPanelManager.Instance.StartLineChart());
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchAnimation() {
        Visualization = VisualizationType.Animation;

        yearHeader.SetActive(true);
        YearPanelManager.Instance.ToggleLineChart(false);
        PriusManager.Instance.TogglePrius(false);
        AnimationManager.Instance.ToggleAnimation(true);

        StartCoroutine(AnimationManager.Instance.StartAnimations());
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        Visualization = VisualizationType.Prius;

        yearHeader.SetActive(true);
        YearPanelManager.Instance.ToggleLineChart(false);
        AnimationManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(true);

        StartCoroutine(PriusManager.Instance.StartPrius());
    }

    /// <summary>
    /// When the slider is changed, update the year.
    /// </summary>
    /// <param name="value">Value.</param>
    public void UpdateYear(int value) {
        Year = value;

        // update header text
        StringBuilder builder = new StringBuilder(Year + " year");
        if (Year > 1) {
            builder.Append("s");
        }

        SliderText.text = builder.ToString();

        builder.Append(" Later (" + Path + ")");
        HeaderText.text = builder.ToString();

        if (Visualization == VisualizationType.Animation) {
            AnimationManager.Instance.Visualize();
        } else if (Visualization == VisualizationType.Prius) {
            PriusManager.Instance.Visualize();
        }
    }

    /// <summary>
    /// When the button is pressed, update the path.
    /// </summary>
    /// <param name="keyword">Keyword.</param>
    public void UpdatePath(string keyword) {
        Path = keyword;
        TutorialText.Instance.Show("Switched to " + keyword, 3);
        if (Visualization == VisualizationType.Animation) {
            AnimationManager.Instance.Visualize();
        } else if (Visualization == VisualizationType.Prius) {
            PriusManager.Instance.Visualize();
        }
    }

    /// <summary>
    /// When "play/pause" button is clicked, start/stop time progression.
    /// </summary>
    public void TimePlayPause() {
        if (Path == null) {
            TutorialText.Instance.ShowDouble("You haven't chosen a path", "Choose a path then continue", 3.0f);
        } else {
            if (!isTimePlaying) { // stopped/paused, start
                timeProgressCoroutine = TimeProgress();
                StartCoroutine(timeProgressCoroutine);
            } else { // started, pause
                StopCoroutine(timeProgressCoroutine);
            }
            isTimePlaying = !isTimePlaying;
            playPauseButton.ChangeImage(isTimePlaying);
        }
    }

    /// <summary>
    /// When "stop" button is clicked, stop and reset time progression.
    /// </summary>
    public void TimeStop() {
        if (isTimePlaying) {
            TimePlayPause();
        }
        UpdateYear(0);
        Interact.SetSlider(0);
    }

    /// <summary>
    /// Helper method to progress through time.
    /// </summary>
    IEnumerator TimeProgress() {
        while (Year <= 25) {
            UpdateYear(Year);
            Interact.SetSlider(((float)Year) / 25);

            yield return new WaitForSeconds(2f);
            Year += 5;
        }
        // after loop, stop.
        isTimePlaying = false;
        playPauseButton.ChangeImage(isTimePlaying);
        yield return null;
    }

    /// <summary>
    /// Jumps to a specific year. Added range checks.
    /// Used for "backward" and "forward" buttons on the control panel.
    /// </summary>
    /// <param name="yearInterval">a year interval, NOT an actual year. For example, 5 means "5 years later".</param>
    public void TimeJump(int yearInterval) {
        if (Path == null) {
            TutorialText.Instance.ShowDouble("You haven't chosen a path", "Choose a path then continue", 3.0f);
        } else {
            int newYear;
            if (yearInterval > 0) {
                newYear = Year + yearInterval > 25 ? 25 : Year + yearInterval;
            } else {
                newYear = Year + yearInterval < 0 ? 0 : Year + yearInterval;
            }

            UpdateYear(newYear);
            TutorialText.Instance.Show("Switched to Year " + Year, 2);
        }
    }

    /// <summary>
    /// Reset year and path values.
    /// </summary>
    public void Reset() {
        Year = 0;
        Path = null;
    }
    #endregion
}
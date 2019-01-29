using System.Collections;
using UnityEngine;
using Collections.Hybrid.Generic;


public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }

    public GameObject stage;
    public GameObject stageObject;
    public GameObject controlPanel;
    public Transform[] positionList;


    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private Vector3 controlPanelInitLocPos;
    private bool isAnimating;

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
                Vector3 stageCenter = stage.transform.GetChild(0).position;
                Vector3 diff = stage.transform.position - stageCenter;
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
        //TODO: remove
        HumanManager.Instance.SelectedHuman.SetActive(true);
        AnimationManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(false);
        YearPanelManager.Instance.ToggleLineChart(true);

        YearPanelManager.Instance.StartLineChart();
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchAnimation() {
        Visualization = VisualizationType.Animation;
        //TODO: remove
        HumanManager.Instance.SelectedHuman.SetActive(false);
        YearPanelManager.Instance.ToggleLineChart(false);
        PriusManager.Instance.TogglePrius(false);
        AnimationManager.Instance.ToggleAnimation(true);

        AnimationManager.Instance.StartAnimations();
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        Visualization = VisualizationType.Prius;
        //TODO: remove
        HumanManager.Instance.SelectedHuman.SetActive(false);
        YearPanelManager.Instance.ToggleLineChart(false);
        AnimationManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(true);

        PriusManager.Instance.StartPrius();
    }



    /// <summary>
    /// When the slider is changed, update the year.
    /// </summary>
    /// <param name="value">Value.</param>
    public void UpdateYear(int value) {
        Year = value;
        if (Visualization == VisualizationType.Animation) {
            AnimationManager.Instance.Visualize();
        } else if (Visualization == VisualizationType.Prius) {

        }
    }

    /// <summary>
    /// When the button is pressed, update the path.
    /// </summary>
    /// <param name="keyword">Keyword.</param>
    public void UpdatePath(string keyword) {
        Path = keyword;
        if (Visualization == VisualizationType.Animation) {
            AnimationManager.Instance.Visualize();
            TutorialText.Instance.Show("Switched to " + keyword, 3);
        } else if (Visualization == VisualizationType.Prius) {

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
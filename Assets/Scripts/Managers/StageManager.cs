using System.Collections;
using System.Collections.Generic;
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
    public GameObject yearHeader;

    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private bool isAnimating;

    public Transform CenterTransform { get { return stage.transform.GetChild(0); } }

    public readonly Dictionary<HealthChoice, string> choicePathDictionary = new Dictionary<HealthChoice, string> {
        {HealthChoice.None, "No Life Plan Change"},
        {HealthChoice.Minimal, "Minimal Change"},
        {HealthChoice.Recommended, "Optimal Change"}
    };

    #region Unity routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
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

        float step = 0.0f;
        float stepLength = 0.01f;

        isAnimating = true;

        Vector3 originalPos = controlPanel.transform.localPosition;
        Vector3 initialPos = new Vector3(controlPanel.transform.localPosition.x, -10f, controlPanel.transform.localPosition.z);
        controlPanel.transform.localPosition = initialPos;

        while (step < 1.0f) {
            controlPanel.transform.localPosition = Vector3.Lerp(originalPos, initialPos, step);
            step += stepLength;
            yield return null;
        }

        controlPanel.transform.localPosition = originalPos;
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
        MasterManager.Instance.CurrGamePhase = GamePhase.VisLineChart;

        yearHeader.SetActive(false);
        ActivityManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(false);
        YearPanelManager.Instance.ToggleLineChart(true);

        StartCoroutine(YearPanelManager.Instance.StartLineChart());
    }

    /// <summary>
    /// When the button is pressed, switch to animations visualization.
    /// </summary>
    public void SwitchActivity() {
        MasterManager.Instance.CurrGamePhase = GamePhase.VisActivity;

        yearHeader.SetActive(true);
        YearPanelManager.Instance.ToggleLineChart(false);
        PriusManager.Instance.TogglePrius(false);
        ActivityManager.Instance.ToggleAnimation(true);
        StartCoroutine(ActivityManager.Instance.StartAnimations());
    }

    /// <summary>
    /// When the button is pressed, switch to prius visualization.
    /// </summary>
    public void SwitchPrius() {
        MasterManager.Instance.CurrGamePhase = GamePhase.VisPrius;

        yearHeader.SetActive(true);
        YearPanelManager.Instance.ToggleLineChart(false);
        ActivityManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(true);

        StartCoroutine(PriusManager.Instance.StartPrius());
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetVisualizations() {
        yearHeader.SetActive(false);
        ActivityManager.Instance.ToggleAnimation(false);
        PriusManager.Instance.TogglePrius(false);
        YearPanelManager.Instance.ToggleLineChart(false);
        // ToggleLineChart will enable line chart button.
        // However, during MasterManager's Reset() a call will be made to ButtonSequenceManager
        // thus automatically resetting all buttons. So no need to worry.
    }
    #endregion
}
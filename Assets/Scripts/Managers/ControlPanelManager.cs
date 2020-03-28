using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ControlPanelManager : MonoBehaviour {
    public static ControlPanelManager Instance { get; private set; }

    public GameObject predictPanel;
    public GameObject lineChartControls;
    public GameObject timeControls;

    public GameObject activitySelector;
    public GameObject priusSelector;
    public GameObject lineChartSelector;


    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void InitializeButtons() {
        TogglePredictPanel(false);
        ToggleLineChartControls(false);
        ToggleTimeControls(false);
        ToggleActivitySelector(false);
        TogglePriusSelector(false);
        ToggleLineChartSelector(false);
    }

    public void TogglePredictPanel(bool on) {
        predictPanel.SetActive(on);
    }

    public void ToggleLineChartControls(bool on) {
        lineChartControls.SetActive(on);
    }

    public void ToggleTimeControls(bool on) {
        timeControls.SetActive(on);
    }

    public void ToggleActivitySelector(bool on) {
        activitySelector.SetActive(on);
    }

    public void TogglePriusSelector(bool on) {
        priusSelector.SetActive(on);
    }
    public void ToggleLineChartSelector(bool on) {
        lineChartSelector.SetActive(on);
    }

    public void Advance() {
        if (MasterManager.Instance.currPhase == GamePhase.Idle) {
            // Awaiting for user input.
            HumanManager.Instance.PrepareVisualization();
            StageManager.Instance.SwitchActivity();
        } else if (MasterManager.Instance.currPhase == GamePhase.VisLineChart) {
            StageManager.Instance.SwitchActivity();
        } else if (MasterManager.Instance.currPhase == GamePhase.VisActivity) {
            StageManager.Instance.SwitchPrius();
        } else if (MasterManager.Instance.currPhase == GamePhase.VisPrius) {
            StageManager.Instance.SwitchLineChart();
        }
    }

    public void ChoosePathNone() {
        TimeProgressManager.Instance.UpdatePath(0);
    }

    public void ChoosePathMinimal() {
        TimeProgressManager.Instance.UpdatePath(1);
    }

    public void ChoosePathOptimal() {
        TimeProgressManager.Instance.UpdatePath(2);
    }
}

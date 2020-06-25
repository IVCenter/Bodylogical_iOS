using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ControlPanelManager : MonoBehaviour {
    public static ControlPanelManager Instance { get; private set; }

    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject settingsPanel;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        InitializeButtons();
    }

    public void InitializeButtons() {
        TogglePredictPanel(false);
        ToggleSettingsPanel(false);
    }

    public void TogglePredictPanel(bool on) {
        controlPanel.SetActive(on);
    }

    public void ToggleSettingsPanel(bool on) {
        settingsPanel.SetActive(on);
    }

    public void Advance() {
        if (AppStateManager.Instance.currState == AppState.Idle) {
            DetailPanelManager.Instance.ToggleDetailPanel(false);
            StageManager.Instance.SwitchActivity();
        } else if (AppStateManager.Instance.currState == AppState.VisLineChart) {
            StageManager.Instance.SwitchActivity();
        } else if (AppStateManager.Instance.currState == AppState.VisActivity) {
            StageManager.Instance.SwitchPrius();
        } else if (AppStateManager.Instance.currState == AppState.VisPrius) {
            StageManager.Instance.SwitchLineChart();
        }
    }

    public void Back() {
        if (AppStateManager.Instance.currState == AppState.VisLineChart) {
            StageManager.Instance.SwitchPrius();
        } else if (AppStateManager.Instance.currState == AppState.VisActivity) {
            StageManager.Instance.SwitchLineChart();
        } else if (AppStateManager.Instance.currState == AppState.VisPrius) {
            StageManager.Instance.SwitchActivity();
        }
    }
}

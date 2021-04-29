using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ControlPanelManager : MonoBehaviour {
    public static ControlPanelManager Instance { get; private set; }

    [SerializeField] private GameObject basicInfoPanel;
    [SerializeField] private GameObject lifestylePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject timelineHandle;

    public BasicInfoPanel DPanel => basicInfoPanel.GetComponent<BasicInfoPanel>();
    public LifestylePanel LPanel => lifestylePanel.GetComponent<LifestylePanel>();
    
    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        ToggleDataPanel(false);
        ToggleControlPanel(false);
        ToggleSettingsPanel(false);
        
        Initialize();
    }

    public void Initialize() {
        ToggleHandle(false);
        DPanel.ResetFields();
        DPanel.LockButtons(false);
        LPanel.ResetFields();
        LPanel.LockButtons(false);
    }

    public void ToggleDataPanel(bool on) {
        basicInfoPanel.SetActive(on);
    }
    
    public void ToggleControlPanel(bool on) {
        lifestylePanel.SetActive(on);
    }

    public void ToggleSettingsPanel(bool on) {
        settingsPanel.SetActive(on);
    }
    
    public void ToggleHandle(bool on) {
        timelineHandle.GetComponent<SliderInteract>().enabled = on;
    }

    public void Advance() {
        // if (AppStateManager.Instance.CurrState == AppState.Visualizations) {
        //     //DetailPanel.Instance.ToggleDetailPanel(false);
        //     StageManager.Instance.SwitchActivity();
        // } else if (AppStateManager.Instance.CurrState == AppState.VisLineChart) {
        //     StageManager.Instance.SwitchActivity();
        // } else if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
        //     StageManager.Instance.SwitchPrius();
        // } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
        //     StageManager.Instance.SwitchLineChart();
        // }
    }

    public void Back() {
        // if (AppStateManager.Instance.CurrState == AppState.VisLineChart) {
        //     StageManager.Instance.SwitchPrius();
        // } else if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
        //     StageManager.Instance.SwitchLineChart();
        // } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
        //     StageManager.Instance.SwitchActivity();
        // }
    }
}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ControlPanelManager : MonoBehaviour {
    public static ControlPanelManager Instance { get; private set; }

    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject[] ribbonHeaders;
    [SerializeField] private GameObject[] interventionButtons;
    [SerializeField] private GameObject[] animationButtons;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject timelineHandle;
    [SerializeField] private Color disabledColor;
    private Color ribbonHeaderColor;
    private Color buttonColor;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        ToggleControlPanel(false);

        ribbonHeaderColor = ribbonHeaders[0].GetComponent<Text>().color;
        buttonColor = GetText(nextButton).color;
        Initialize();
    }

    public void Initialize() {
        ToggleRibbonAccess(false);
        ToggleInterventions(false);
        ToggleAnimations(false);
        ToggleNext(false);
        TogglePrev(false);
        ToggleHandle(false);
    }

    public void ToggleControlPanel(bool on) {
        controlPanel.SetActive(on);
    }

    public void ToggleRibbonAccess(bool on) {
        foreach (GameObject header in ribbonHeaders) {
            header.GetComponent<ButtonInteract>().enabled = on;
            header.GetComponent<Text>().color = on ? ribbonHeaderColor : disabledColor;
        }
    }

    public void ToggleInterventions(bool on) {
        foreach (GameObject button in interventionButtons) {
            button.GetComponent<ButtonInteract>().enabled = on;
            GetText(button).color = on ? buttonColor : disabledColor;
        }
    }

    public void ToggleAnimations(bool on) {
        foreach (GameObject button in animationButtons) {
            button.GetComponent<ButtonInteract>().enabled = on;
            GetText(button).color = on ? buttonColor : disabledColor;
        }
    }

    public void ToggleNext(bool on) {
        nextButton.GetComponent<ButtonInteract>().enabled = on;
        GetText(nextButton).color = on ? buttonColor : disabledColor;
    }

    public void TogglePrev(bool on) {
        prevButton.GetComponent<ButtonInteract>().enabled = on;
        GetText(prevButton).color = on ? buttonColor : disabledColor;
    }

    public void ToggleHandle(bool on) {
        timelineHandle.GetComponent<SliderInteract>().enabled = on;
    }

    public void Advance() {
        if (AppStateManager.Instance.CurrState == AppState.Idle) {
            DetailPanelManager.Instance.ToggleDetailPanel(false);
            StageManager.Instance.SwitchActivity();
        } else if (AppStateManager.Instance.CurrState == AppState.VisLineChart) {
            StageManager.Instance.SwitchActivity();
        } else if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
            StageManager.Instance.SwitchPrius();
        } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
            StageManager.Instance.SwitchLineChart();
        }
    }

    public void Back() {
        if (AppStateManager.Instance.CurrState == AppState.VisLineChart) {
            StageManager.Instance.SwitchPrius();
        } else if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
            StageManager.Instance.SwitchLineChart();
        } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
            StageManager.Instance.SwitchActivity();
        }
    }

    private Text GetText(GameObject obj) {
        return obj.transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }
}
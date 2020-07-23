using System.Collections;
using UnityEngine;

public class StartPanelManager : MonoBehaviour {
    public static StartPanelManager Instance { get; private set; }

    [SerializeField] private GameObject startPanel;
    [SerializeField] private LocalizedText languageButtonText;
    [SerializeField] private LocalizedText tutorialButtonText;

    private IEnumerator positionCoroutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        startPanel.SetActive(true);
        startPanel.transform.SetParent(Camera.main.transform);
        positionCoroutine = PositionPanel();
        StartCoroutine(positionCoroutine);
    }

    private IEnumerator PositionPanel() {
        Vector3 forward = new Vector3(0, -0.075f, 0);
        forward.z = DeviceManager.Instance.Constants.startScreenDistance;
        
        while (true) {
            startPanel.transform.localPosition = forward;
            yield return null;
        }
    }

    public void Confirm() {
        if (Application.isEditor) {
            StageManager.Instance.ToggleStage(true);
            ArchetypeManager.Instance.LoadArchetypes();

            AppStateManager.Instance.CurrState = AppState.PlaceStage;
        } else {
            AppStateManager.Instance.CurrState = AppState.FindPlane;
            PlaneManager.Instance.BeginScan();
            TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        }
        startPanel.SetActive(false);
        StopCoroutine(positionCoroutine);
    }

    /// <summary>
    /// Changes the text for the language button.
    /// </summary>
    /// <param name="id">Index for the language, defined in <see cref="Language"/>.</param>
    public void ToggleLanguage(int id) {
        languageButtonText.SetText("Buttons.Language",
            new LocalizedParam("General.Lang-" + ((Language)id).ToString(), true));
    }

    public void ToggleTutorialSkip(bool on) {
        TutorialManager.Instance.SkipAll = !on; // DO NOT skip when we want tutorials
        if (on) { // shows tutorials
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOn", true));
            ActivityManager.Instance.TutorialShown = false;
            LineChartManager.Instance.TutorialShown = false;
            PriusManager.Instance.TutorialShown = false;
        } else {
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOff", true));
            TutorialManager.Instance.ClearTutorial();
        }
    }

}

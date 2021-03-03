using System.Collections;
using UnityEngine;

public class StartPanel : MonoBehaviour {
    [SerializeField] private LocalizedText languageButtonText;
    [SerializeField] private LocalizedText tutorialButtonText;

    private bool confirmed;

    private void Start() {
        gameObject.SetActive(true);
        gameObject.transform.SetParent(Camera.main.transform);
        StartCoroutine(PositionPanel());
    }

    private IEnumerator PositionPanel() {
        Vector3 forward = new Vector3(0, -0.075f, 0);
        forward.z = DeviceManager.Instance.Constants.startScreenDistance;

        while (!confirmed) {
            transform.localPosition = forward;
            yield return null;
        }
    }

    public void Confirm() {
        if (Application.isEditor) {
            StageManager.Instance.ToggleStage(true);
            // Nothing is selected yet, so this will show all displayers.
            ArchetypeManager.Instance.ToggleUnselectedDisplayers(true);
            AppStateManager.Instance.CurrState = AppState.PlaceStage;
        } else {
            AppStateManager.Instance.CurrState = AppState.FindPlane;
            PlaneManager.Instance.BeginScan();
        }

        gameObject.SetActive(false);
        confirmed = true;
    }

    /// <summary>
    /// Changes the text for the language button.
    /// </summary>
    /// <param name="id">Index for the language, defined in <see cref="Language"/>.</param>
    public void ToggleLanguage(int id) {
        languageButtonText.SetText("Buttons.Language",
            new LocalizedParam($"General.Lang-{(Language) id}", true));
    }

    public void ToggleTutorialSkip(bool on) {
        TutorialManager.Instance.SkipAll = !on; // DO NOT skip when we want tutorials
        if (on) { // shows tutorials
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOn", true));
            // Reset all tutorials
            //ActivityManager.Instance.TutorialShown = false;
            //LineChartManager.Instance.TutorialShown = false;
            //PriusManager.Instance.TutorialShown = false;
        } else {
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOff", true));
            TutorialManager.Instance.ClearTutorial();
        }
    }
}
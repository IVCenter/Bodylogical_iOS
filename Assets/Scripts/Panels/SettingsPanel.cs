using UnityEngine;

/// <summary>
/// Functions for the settings panel.
/// </summary>
public class SettingsPanel : MonoBehaviour {
    [SerializeField] private LocalizedText tutorialButtonText, languageButtonText, unitButtonText;

    public void ExitGame() {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor.
        // UnityEditor.EditorApplication.isPlaying need to be set to false to exit the game.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToggleTutorialSkip(bool on) {
        TutorialManager.Instance.SkipAll = !on; // DO NOT skip when we want tutorials
        if (on) { // shows tutorials
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOn", true));
            //ActivityManager.Instance.TutorialShown = false;
            //LineChartManager.Instance.TutorialShown = false;
            //PriusManager.Instance.TutorialShown = false;
        } else {
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOff", true));
            TutorialManager.Instance.ClearTutorial();
        }
    }

    /// <summary>
    /// Changes the text for the language button.
    /// </summary>
    /// <param name="id">Index for the language, defined in <see cref="Language"/>.</param>
    public void ToggleLanguage(int id) {
        Language lang = (Language) id;
        languageButtonText.SetText("Buttons.Language", new LocalizedParam($"General.Lang-{lang}", true));
        LocalizationManager.Instance.ChangeLanguage(lang);
    }

    public void ToggleUnit(int id) {
        Unit unit = (Unit) id;
        unitButtonText.SetText("Buttons.Unit", new LocalizedParam($"General.{unit}", true));
        UnitManager.Instance.ChangeUnit(unit);
    }
}
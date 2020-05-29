using UnityEngine;

public class SettingsPanel : MonoBehaviour {
    [SerializeField] LocalizedText tutorialButtonText, languageButtonText;

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
        TutorialManager.Instance.skipAll = !on; // DO NOT skip when we want tutorials
        if (on) { // shows tutorials
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOn", true));
            ActivityManager.Instance.tutorialShown = false;
            LineChartManager.Instance.tutorialShwon = false;
            PriusManager.Instance.tutorialShown = false;
        } else {
            tutorialButtonText.SetText("Buttons.Tutorial", new LocalizedParam("Buttons.ToggleOff", true));
            TutorialManager.Instance.ClearTutorial();
        }
    }

    /// <summary>
    /// Changes the text for the language button.
    /// </summary>
    /// <param name="langID">Index for the language, defined in <see cref="Language"/>.</param>
    public void ToggleLanguage(int id) {
        languageButtonText.SetText("Buttons.Language",
            new LocalizedParam("General.Lang-" + ((Language)id).ToString(), true));
    }
}

using UnityEngine;

public class SettingsPanel : MonoBehaviour {
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
        TutorialManager.Instance.skipAll = !on;
        if (on) { // shows tutorials
            ActivityManager.Instance.tutorialShown = false;
            LineChartManager.Instance.tutorialShwon = false;
            PriusManager.Instance.tutorialShown = false;
        }
    }
}

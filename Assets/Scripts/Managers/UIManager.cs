using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    #region Welcome screen
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject confirmButton;

    public void SelectLanguage(int lang) {
        LocalizationManager.Instance.ChangeLanguage(lang);
        confirmButton.SetActive(true);
    }

    /// <summary>
    /// TODO: redundant code. Awaiting refactoring.
    /// </summary>
    public void ConfirmLanguage() {
#if UNITY_EDITOR
        AppStateManager.Instance.currState = AppState.PickArchetype;

        StageManager.Instance.ToggleStage(true);
        ArchetypeManager.Instance.LoadArchetypes();
        TutorialManager.Instance.ClearInstruction();
        StageManager.Instance.HideStageObject();
        ArchetypeManager.Instance.SetIdlePose();

        // This will be the first time the user uses the interaction system.
        // So a tutorial is added here.
        TutorialParam content = new TutorialParam(
                "Tutorials.InteractionTitle", "Tutorials.InteractionText");
        TutorialManager.Instance.ShowTutorial(content, TutorialManager.Instance.tutorialParent);

        AppStateManager.Instance.currState = AppState.PickArchetype;
#else
        AppStateManager.Instance.currState = AppState.FindPlane;
        PlaneManager.Instance.BeginScan();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
#endif
        startCanvas.SetActive(false);
        InputManager.Instance.menuOpened = false;
    }
    #endregion

    #region Pause menu
    public GameObject pauseCanvas;
    public Toggle tutorialSkipToggle;
    private bool pauseScreenOn;

    public void TogglePauseMenu() {
        pauseScreenOn = !pauseScreenOn;
        InputManager.Instance.menuOpened = pauseScreenOn;
        pauseCanvas.SetActive(pauseScreenOn);
        tutorialSkipToggle.isOn = !TutorialManager.Instance.skipAll;
        Time.timeScale = pauseScreenOn ? 0 : 1;
    }

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
            StageManager.Instance.ResetTutorial();
        }
    }

    public void Reset() {
        AppStateManager.Instance.Reset();
    }
    #endregion
}

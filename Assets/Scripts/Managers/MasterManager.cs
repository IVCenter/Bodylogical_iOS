using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager that controls the game phases.
/// </summary>
public class MasterManager : MonoBehaviour {
    public static MasterManager Instance { get; private set; }

    private bool stageReady;
    private bool stageBuilt;

    [HideInInspector]
    public GamePhase currPhase;
    [HideInInspector]
    public GameObject particleObj;

    #region Unity Routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        currPhase = GamePhase.FindPlane;
        stageReady = false;
    }

    void Start() {
        ButtonSequenceManager.Instance.InitializeButtons();
        StartCoroutine(GameRunning());
    }
    #endregion

    #region Phases
    /// <summary>
    /// No reset in ChooseLanguage or FindPlane
    /// When the game is after PickArchetype: reset to PickArchetype
    /// When the game is in PickArchetype: reset to FindPlane
    /// </summary>
    public void ResetGame() {
        if (currPhase == GamePhase.PickArchetype) { // reset to FindPlane
            stageReady = false;
            StageManager.Instance.DisableStage();
            StageManager.Instance.DisableControlPanel();
            HumanManager.Instance.StartSelectHuman = false;
            PlaneManager.Instance.RestartScan();

            currPhase = GamePhase.FindPlane;
        } else if (currPhase != GamePhase.ChooseLanguage) { // reset to PickArchetype
            // Reset Activity, YearPanel and Prius
            TimeProgressManager.Instance.Reset();
            StageManager.Instance.ResetVisualizations();
            // In Year Panel the ribbon charts need to be destroyed.
            YearPanelManager.Instance.Reset();
            // In Activity the default activity should be reset.
            ActivityManager.Instance.Reset();
            // Hide detail/choice panel
            DetailPanelManager.Instance.ToggleDetailPanel(false);
            ChoicePanelManager.Instance.ToggleChoicePanels(false);
            // Reset current archetype
            HumanManager.Instance.Reset();

            currPhase = GamePhase.PickArchetype;
        }

        // Common functions
        ButtonSequenceManager.Instance.InitializeButtons();
    }

    /// <summary>
    /// Main game loop.
    /// </summary>
    /// <returns>The running.</returns>
    IEnumerator GameRunning() {
        while (true) {
            switch (currPhase) {
                case GamePhase.FindPlane:
                    yield return CheckPlane();
                    break;
                case GamePhase.PlaceStage:
                    yield return ConfirmStage();
                    break;
                case GamePhase.PickArchetype:
                    yield return SelectArchetype();
                    break;
                case GamePhase.ShowDetails:
                    yield return ShowInfo();
                    break;
                default:
                    yield return Idle();
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// After finding a suitable plane, switch to phase 2.
    /// </summary>
    IEnumerator CheckPlane() {
        if (PlaneManager.Instance.PlaneFound) {
            currPhase = GamePhase.PlaceStage;
        }

        yield return null;
    }

    /// <summary>
    /// Prompts the user to confirm the stage.
    /// </summary>
    IEnumerator ConfirmStage() {
        if (!stageReady) {
            TutorialManager.Instance.ShowInstruction("Instructions.StageCreate");
            yield return new WaitForSeconds(1.0f);

            if (particleObj != null) {
                particleObj.SetActive(false);
            }

            if (!stageBuilt) {
                stageBuilt = true; // only needs to build once
                StageManager.Instance.BuildStage();
            }
            stageReady = true;
        }

        StageManager.Instance.UpdateStageTransform();

        TutorialManager.Instance.ShowInstruction("Instructions.StageConfirm");

        if ((InputManager.Instance.TouchCount > 0 && Input.GetTouch(0).tapCount >= 2) || Input.GetKeyDown("space")) {
            TutorialManager.Instance.ClearInstruction();
            StageManager.Instance.SettleStage();
            PlaneManager.Instance.HideMainPlane();
            StageManager.Instance.EnableControlPanel();
            StageManager.Instance.SetHumanIdlePose();
            currPhase = GamePhase.PickArchetype;
        }

        yield return null;
    }

    /// <summary>
    /// The user needs to select a human model.
    /// When the human model is selected, put it into the center of the stage.
    /// Also, needs to set the model in the props view.
    /// </summary>
    IEnumerator SelectArchetype() {
        if (!HumanManager.Instance.StartSelectHuman && !HumanManager.Instance.IsHumanSelected) {
            TutorialManager.Instance.ShowInstruction("Instructions.ArchetypeSelect");
            HumanManager.Instance.StartSelectHuman = true;
        }

        if (HumanManager.Instance.IsHumanSelected) {
            TutorialManager.Instance.ClearInstruction();
            // move model to center
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            yield return new WaitForSeconds(0.5f);

            currPhase = GamePhase.ShowDetails;
        }

        yield return null;
    }

    /// <summary>
    /// The model stands out. Now showing the basic information
    /// </summary>
    IEnumerator ShowInfo() {
        TutorialManager.Instance.ShowInstruction("Instructions.ArchetypeRead");
        yield return new WaitForSeconds(0.5f);
        HumanManager.Instance.ToggleUnselectedHuman(false);
        HumanManager.Instance.ToggleInteraction(false);
        TutorialManager.Instance.ClearInstruction();
        HumanManager.Instance.ExpandSelectedHumanInfo();
        YearPanelManager.Instance.LoadBounds(); // load data specific to the human body to the year panel
        YearPanelManager.Instance.LoadValues();
        TutorialManager.Instance.ShowStatus("Instructions.ArchetypePredict");
        ButtonSequenceManager.Instance.SetPredictButton(true);
        currPhase = GamePhase.Idle;

        yield return null;
    }

    /// <summary>
    /// Interaction will be triggered when certain buttons on the control panel is clicked.
    /// Nothing to be done here right now.
    /// </summary>
    IEnumerator Idle() {
        yield return null;
    }
    #endregion

    #region Welcome screen
    public GameObject startCanvas;

    public void SelectLanguage(int lang) {
        LocalizationManager.Instance.ChangeLanguage(lang);
        currPhase = GamePhase.FindPlane;
        PlaneManager.Instance.finding = true;
        startCanvas.SetActive(false);
        InputManager.Instance.menuOpened = false;
    }
    #endregion

    #region Pause menu
    public GameObject pauseCanvas;
    private bool pauseScreenOn;
    public Toggle tutorialSkipToggle;

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
    }
    #endregion
}

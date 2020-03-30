using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the state machine transitions.
/// This script does NOT store any external information other than the current
/// state.
/// </summary>
public class AppStateManager : MonoBehaviour {
    public static AppStateManager Instance { get; private set; }

    [HideInInspector] public AppState currState = AppState.ChooseLanguage;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        StartCoroutine(Run());
    }

    /// <summary>
    /// Main loop that performs different actions based on the current app state.
    /// </summary>
    private IEnumerator Run() {
        while (true) {
            switch (currState) {
                case AppState.FindPlane:
                    yield return CheckPlane();
                    break;
                case AppState.PlaceStage:
                    yield return ConfirmStage();
                    break;
                case AppState.PickArchetype:
                    yield return SelectArchetype();
                    break;
                case AppState.ShowDetails:
                    yield return ShowInfo();
                    break;
                default:
                    yield return Idle();
                    break;
            }
        }
    }

    /// <summary>
    /// Tries to find a plane that is large enough for the stage.
    /// </summary>
    private IEnumerator CheckPlane() {
        if (PlaneManager.Instance.PlaneFound) {
            currState = AppState.PlaceStage;
        }

        yield return null;
    }

    /// <summary>
    /// Prompts the user to place the stage.
    /// </summary>
    private IEnumerator ConfirmStage() {
        if (!StageManager.Instance.stageReady) {
            TutorialManager.Instance.ShowInstruction("Instructions.StageCreate");
            yield return new WaitForSeconds(1.0f);

            ArchetypeManager.Instance.LoadArchetypes();
            StageManager.Instance.stageReady = true;
            StageManager.Instance.ToggleStage(true);
        }

        StageManager.Instance.UpdateStageTransform();

        TutorialManager.Instance.ShowInstruction("Instructions.StageConfirm");

        if (InputManager.Instance.TouchCount > 0 && InputManager.Instance.TapCount >= 2) {
            TutorialManager.Instance.ClearInstruction();
            StageManager.Instance.HideStageObject();
            PlaneManager.Instance.HideMainPlane();
            ArchetypeManager.Instance.SetIdlePose();

            // This will be the first time the user uses the interaction system,
            // so a tutorial is added here.
            TutorialParam content = new TutorialParam(
                "Tutorials.InteractionTitle", "Tutorials.InteractionText");
            TutorialManager.Instance.ShowTutorial(content);

            currState = AppState.PickArchetype;
        }

        yield return null;
    }

    /// <summary>
    /// The user needs to select an archetype.
    /// When the archetype is selected, place into the center of the stage.
    /// </summary>
    private IEnumerator SelectArchetype() {
        if (!ArchetypeManager.Instance.startSelectArchetype && !ArchetypeManager.Instance.archetypeSelected) {
            TutorialManager.Instance.ShowInstruction("Instructions.ArchetypeSelect");
            ArchetypeManager.Instance.startSelectArchetype = true;
        }

        if (ArchetypeManager.Instance.archetypeSelected) {
            TutorialManager.Instance.ClearInstruction();
            // move model to center
            yield return ArchetypeManager.Instance.MoveSelectedArchetypeToCenter();
            yield return new WaitForSeconds(0.5f);

            currState = AppState.ShowDetails;
        }

        yield return null;
    }

    /// <summary>
    /// After the archetype is selected, her information needs to be loaded from
    /// the data file and displayed on the panels.
    /// </summary>
    private IEnumerator ShowInfo() {
        TutorialManager.Instance.ShowInstruction("Instructions.ArchetypeRead");
        yield return new WaitForSeconds(0.5f);
        ArchetypeManager.Instance.ToggleUnselectedArchetype(false);
        TutorialManager.Instance.ClearInstruction();
        ArchetypeManager.Instance.ExpandArchetypeInfo();
        LineChartManager.Instance.LoadBounds(); // load the archetype's data to the line chart
        LineChartManager.Instance.LoadValues();
        TutorialManager.Instance.ShowStatus("Instructions.ArchetypePredict");
        ControlPanelManager.Instance.TogglePredictPanel(true);
        currState = AppState.Idle;
        yield return null;
    }

    /// <summary>
    /// Interaction will be triggered when certain buttons on the control panel is clicked.
    /// Nothing to be done here right now.
    /// </summary>
    private IEnumerator Idle() {
        yield return null;
    }

    /// <summary>
    /// No reset in ChooseLanguage or FindPlane
    /// When the game is after PickArchetype: reset to PickArchetype
    /// When the game is in PickArchetype: reset to FindPlane
    /// </summary>
    public void Reset() {
        if (currState == AppState.PickArchetype || currState == AppState.PlaceStage) { // reset to FindPlane
            StageManager.Instance.Reset();
            ArchetypeManager.Instance.startSelectArchetype = false;
            PlaneManager.Instance.RestartScan();

            currState = AppState.FindPlane;
        } else if (currState != AppState.ChooseLanguage) { // reset to PickArchetype
            TimeProgressManager.Instance.Reset();
            StageManager.Instance.ResetVisualizations();
            LineChartManager.Instance.Reset();
            ActivityManager.Instance.Reset();
            PriusManager.Instance.Reset();
            DetailPanelManager.Instance.ToggleDetailPanel(false);
            ChoicePanelManager.Instance.ToggleChoicePanels(false);
            ArchetypeManager.Instance.Reset();

            currState = AppState.PickArchetype;
        }

        ControlPanelManager.Instance.InitializeButtons();
    }
}
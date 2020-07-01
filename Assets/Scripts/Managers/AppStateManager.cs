using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the state machine transitions.
/// This script does NOT store any external information other than the current
/// state.
/// </summary>
public class AppStateManager : MonoBehaviour {
    public static AppStateManager Instance { get; private set; }
    public AppState CurrState { get; set; } = AppState.ChooseLanguage;

    [SerializeField] private Transform interactionTutorialTransform;

    private AppState? stateBeforeReset;

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
            switch (CurrState) {
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
            if (!StageManager.Instance.stageReady) {
                TutorialManager.Instance.ShowInstruction("Instructions.StageCreate");
                yield return new WaitForSeconds(1.0f);

                ArchetypeManager.Instance.LoadArchetypes();
                StageManager.Instance.stageReady = true;
                StageManager.Instance.ToggleStage(true);
            }

            CurrState = AppState.PlaceStage;
        }

        yield return null;
    }

    /// <summary>
    /// Prompts the user to place the stage.
    /// </summary>
    private IEnumerator ConfirmStage() {
        StageManager.Instance.UpdateStageTransform();

        TutorialManager.Instance.ShowInstruction("Instructions.StageConfirm");

        if (Application.isEditor || (InputManager.Instance.TouchCount > 0 && InputManager.Instance.TapCount >= 2)) {
            TutorialManager.Instance.ClearInstruction();
            StageManager.Instance.HideStageObject();
            PlaneManager.Instance.HidePlanes();
            // Show up control panel
            ControlPanelManager.Instance.TogglePredictPanel(true);
            if (stateBeforeReset == null) {
                // First time running
                ArchetypeManager.Instance.SetGreetingPoses(true);

                // This will be the first time the user uses the interaction system,
                // so a tutorial is added here.
                TutorialParam content = new TutorialParam(
                    "Tutorials.InteractionTitle", "Tutorials.InteractionText");
                TutorialManager.Instance.ShowTutorial(content,
                    interactionTutorialTransform,
                    () => ArchetypeManager.Instance.ArchetypeSelected);

                CurrState = AppState.PickArchetype;
            }
            else {
                CurrState = stateBeforeReset.Value;
            }
        }

        yield return null;
    }

    /// <summary>
    /// The user needs to select an archetype.
    /// When the archetype is selected, place to the center of the stage.
    /// </summary>
    private IEnumerator SelectArchetype() {
        if (!ArchetypeManager.Instance.StartSelectArchetype && !ArchetypeManager.Instance.ArchetypeSelected) {
            TutorialManager.Instance.ShowInstruction("Instructions.ArchetypeSelect");
            ArchetypeManager.Instance.StartSelectArchetype = true;
        }

        if (ArchetypeManager.Instance.ArchetypeSelected) {
            TutorialManager.Instance.ClearInstruction();
            // Hide information panel
            ArchetypeManager.Instance.Selected.InfoCanvas.SetActive(false);
            // Move model to center
            ArchetypeManager.Instance.SetGreetingPoses(false);
            yield return ArchetypeManager.Instance.MoveSelectedToCenter();
            yield return new WaitForSeconds(0.5f);

            CurrState = AppState.ShowDetails;
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
        CurrState = AppState.Idle;
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
    /// Let the user select another archetype.
    /// </summary>
    public void ResetAvatar() {
        // Need to be a state after PickArchetype
        if (CurrState != AppState.PickArchetype
            && CurrState != AppState.PlaceStage
            && CurrState != AppState.ChooseLanguage) {
            ControlPanelManager.Instance.InitializeButtons();
            ControlPanelManager.Instance.TogglePredictPanel(true);
            TimeProgressManager.Instance.Reset();
            StageManager.Instance.ResetVisualizations();
            LineChartManager.Instance.Reset();
            ActivityManager.Instance.Reset();
            PriusManager.Instance.Reset();
            DetailPanelManager.Instance.ToggleDetailPanel(false);
            ChoicePanelManager.Instance.ToggleChoicePanels(false);
            ArchetypeManager.Instance.Reset();
            TutorialManager.Instance.ClearTutorial();

            CurrState = AppState.PickArchetype;
        }
    }


    /// <summary>
    /// Lets the user find another plane to place the stage.
    /// </summary>
    public void ResetStage() {
        stateBeforeReset = CurrState;
        ControlPanelManager.Instance.InitializeButtons();
        StageManager.Instance.Reset();
        PlaneManager.Instance.RestartScan();
        TutorialManager.Instance.ClearTutorial();

        CurrState = AppState.FindPlane;
    }
}
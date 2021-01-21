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
    [SerializeField] private Transform panelTutorialTransform;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        // Fix frame rate for character animations
        Application.targetFrameRate = 60;
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
        if (PlaneManager.Instance.PlaneConfirmed) {
            if (!StageManager.Instance.StageReady) {
                TutorialManager.Instance.ShowInstruction("Instructions.StageCreate");
                yield return new WaitForSeconds(1.0f);

                ArchetypeManager.Instance.LoadArchetypes();
                StageManager.Instance.StageReady = true;
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
        bool stageConfirmed = false;
        
        if (PlaneManager.Instance.useImageTracking) {
            StageManager.Instance.CopyTransform();
            stageConfirmed = true;
        } else {
            StageManager.Instance.UpdateStageTransform();
            if (!Application.isEditor) {
                TutorialManager.Instance.ShowInstruction("Instructions.StageConfirm");
                TutorialParam param = new TutorialParam("Tutorials.StageTitle", "Tutorials.StageText");
                // The stage will always follow the camera, so we set the mode to None
                TutorialManager.Instance.ShowTutorial(param, interactionTutorialTransform, () => stageConfirmed,
                    mode: TutorialRemindMode.None);
            }

            if (Application.isEditor || InputManager.Instance.TouchCount > 0 && InputManager.Instance.TapCount >= 2) {
                stageConfirmed = true;
                //TutorialManager.Instance.ClearInstruction();
            }
        }

        if (stageConfirmed) {
            StageManager.Instance.HideStageObject();
            PlaneManager.Instance.HidePlanes();
            
            DebugText.Instance.Log(StageManager.Instance.stage.transform.localPosition.ToString());
            DebugText.Instance.Log(StageManager.Instance.stage.transform.localScale.ToString());

            // Show up control panel
            ControlPanelManager.Instance.ToggleControlPanel(true);
            // First time running
            ArchetypeManager.Instance.SetGreetingPoses(true);

            // This will be the first time the user uses the interaction system,
            // so a tutorial is added here.
            TutorialParam content = new TutorialParam(
                "Tutorials.InteractionTitle", "Tutorials.InteractionText");
            TutorialManager.Instance.ShowTutorial(content, interactionTutorialTransform,
                () => ArchetypeManager.Instance.Selected != null);

            CurrState = AppState.PickArchetype;
        }

        yield return null;
    }

    /// <summary>
    /// The user needs to select an archetype.
    /// When the archetype is selected, place to the center of the stage.
    /// </summary>
    private IEnumerator SelectArchetype() {
        if (!ArchetypeManager.Instance.StartSelectArchetype) {
            TutorialManager.Instance.ShowInstruction("Instructions.ArchetypeSelect");
            ArchetypeManager.Instance.StartSelectArchetype = true;
        }

        if (ArchetypeManager.Instance.Selected != null) {
            TutorialManager.Instance.ClearInstruction();
            ArchetypeManager.Instance.ToggleUnselectedArchetype(false);
            // Move model to center
            ArchetypeManager.Instance.SetGreetingPoses(false);
            yield return ArchetypeManager.Instance.MoveSelectedTo(StageManager.Instance.stageCenter.position);
            yield return new WaitForSeconds(0.5f);
            ArchetypeManager.Instance.Selected.Header.SetMeet();
            ControlPanelManager.Instance.ToggleNext(true); // Enable "Next" button

            ArchetypeManager.Instance.StartSelectArchetype = false;
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
        TutorialManager.Instance.ClearInstruction();
        
        ArchetypeManager.Instance.CreateModels();
        ArchetypeManager.Instance.Selected.Panel.SetValues(ArchetypeManager.Instance.Performers[HealthChoice.None].ArchetypeLifestyle);
        ArchetypeManager.Instance.Selected.Panel.ToggleDetailPanel(true);
        TutorialManager.Instance.ShowStatus("Instructions.ArchetypePredict");

        TutorialParam param = new TutorialParam("Tutorials.ControlTitle", "Tutorials.ControlText");
        // TutorialManager.Instance.ShowTutorial(param, panelTutorialTransform,
        //     () => CurrState == AppState.VisActivity);
        CurrState = AppState.Visualizations;
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
        if (CurrState != AppState.PickArchetype && CurrState != AppState.PlaceStage 
                                                && CurrState != AppState.ChooseLanguage) {
            ControlPanelManager.Instance.Initialize();
            TimeProgressManager.Instance.Reset();
            StageManager.Instance.ResetVisualizations();
            //LineChartManager.Instance.Reset();
            //ActivityManager.Instance.Reset();
            //PriusManager.Instance.ResetManager();
            //DetailPanel.Instance.ToggleDetailPanel(false);
            //ChoicePanelManager.Instance.ToggleChoicePanels(false);
            ArchetypeManager.Instance.Reset();
            TutorialManager.Instance.ClearTutorial();

            CurrState = AppState.PickArchetype;
        }
    }
    
    /// <summary>
    /// Lets the user find another plane to place the stage.
    /// Will also reset the avatar because of tutorial placement issues.
    /// </summary>
    public void ResetStage() {
        ResetAvatar();
        ControlPanelManager.Instance.ToggleControlPanel(false);
        ControlPanelManager.Instance.ToggleSettingsPanel(false);
        TutorialManager.Instance.ClearTutorial();

        if (Application.isEditor) {
            CurrState = AppState.PlaceStage;
        } else {
            StageManager.Instance.ResetStage();
            PlaneManager.Instance.RestartScan();
            CurrState = AppState.FindPlane;
        }
    }
}
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

                // Nothing is selected yet, so this will show all displayers.
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
                // TODO: condition is always false
                TutorialManager.Instance.ShowTutorial(param, interactionTutorialTransform, () => stageConfirmed,
                    mode: TutorialRemindMode.None);
            }

            if (Application.isEditor || InputManager.Instance.TouchCount > 0 && InputManager.Instance.TapCount >= 2) {
                stageConfirmed = true;
                TutorialManager.Instance.ClearInstruction();
            }
        }

        if (stageConfirmed) {
            StageManager.Instance.HideStageObject();
            PlaneManager.Instance.HidePlanes();

            DebugText.Instance.Log(StageManager.Instance.stage.transform.localPosition.ToString());
            DebugText.Instance.Log(StageManager.Instance.stage.transform.localScale.ToString());

            // Show up data panel to allow user input
            ControlPanelManager.Instance.ToggleDataPanel(true);

            CurrState = AppState.Idle;
        }

        yield return null;
    }

    /// <summary>
    /// After confirming the avatar's basic stats, we need to query the server for health data.
    /// </summary>
    private IEnumerator ShowInfo() {
        // Lock the buttons and show a loading text
        ControlPanelManager.Instance.DPanel.LockButtons(true);
        TutorialManager.Instance.ShowInstruction("Instructions.CalculateData");
        ArchetypeManager.Instance.displayer.SetGreetingPose(true);

        // Connect to the API and retrieve the data
        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            performer.Initialize();
        }

        NetworkError error = new NetworkError();

        while (error.status != NetworkStatus.Success) {
            yield return NetworkUtils.UserMatch(ArchetypeManager.Instance.displayer.ArchetypeData,
                ArchetypeManager.Instance.Performer.ArchetypeHealth, error);

            if (error.status == NetworkStatus.ServerError) {
                Debug.Log(error.message);
                TutorialManager.Instance.ShowInstruction(error.MsgKey);
                continue;
            }

            if (error.status == NetworkStatus.Success) {
                break;
            }

            // Request error
            ControlPanelManager.Instance.DPanel.LockButtons(false);
            ArchetypeManager.Instance.displayer.SetGreetingPose(false);
            StartCoroutine(ShowErrorInstruction(error));
            CurrState = AppState.Idle;
            yield break;
        }

        // We don't need the health data of the other two "profile/intervention"s right away, so we will do them
        // asynchronously.
        ArchetypeManager.Instance.SyncArchetype();

        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            if (performer.choice != HealthChoice.Custom) {
                StartCoroutine(performer.QueryHealth(new NetworkError(), null, true));
            } else {
                performer.DataReady = true;
            }
        }

        // Unlock the buttons and hide loading text
        ControlPanelManager.Instance.DPanel.LockButtons(false);
        TutorialManager.Instance.ClearInstruction();

        // Show the data on the panel
        ArchetypeManager.Instance.displayer.panel.SetValues(ArchetypeManager.Instance.Performer.ArchetypeHealth);
        ArchetypeManager.Instance.displayer.panel.Toggle(true);

        ArchetypeManager.Instance.displayer.SetGreetingPose(false);
        ArchetypeManager.Instance.LifestyleTutorial();
        CurrState = AppState.Idle;
        yield return null;
    }

    private IEnumerator ShowErrorInstruction(NetworkError error) {
        Debug.Log(error.message);
        TutorialManager.Instance.ShowInstruction(error.MsgKey);
        yield return new WaitForSeconds(5);
        TutorialManager.Instance.ClearInstruction();
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
        // TODO
        if (CurrState != AppState.Idle && CurrState != AppState.PlaceStage &&
            CurrState != AppState.ChooseLanguage) {
            ControlPanelManager.Instance.Initialize();
            TimeProgressManager.Instance.ResetTime();
            StageManager.Instance.ResetVisualizations();
            ArchetypeManager.Instance.ResetAvatars();
            TutorialManager.Instance.ClearTutorial();

            CurrState = AppState.Idle;
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
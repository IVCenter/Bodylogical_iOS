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

    // Used for resetting avatar.
    private IEnumerator showInfoCoroutine;

    private bool stageConfirmed;

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
                    yield return showInfoCoroutine = ShowInfo();
                    break;
                default:
                    yield return Idle();
                    break;
            }
        }
    }

    /// <summary>
    /// When the plane has been confirmed, enable the stage and prompts the user to place it on the plane.
    /// </summary>
    private IEnumerator CheckPlane() {
        if (PlaneManager.Instance.PlaneConfirmed) {
            if (!StageManager.Instance.StageReady) {
                TutorialManager.Instance.ShowInstruction("Instructions.StageCreate");
                yield return new WaitForSeconds(1.0f);

                StageManager.Instance.StageReady = true;
                StageManager.Instance.ToggleStage(true);
            }

            stageConfirmed = false;
            TutorialManager.Instance.ShowInstruction("Instructions.StageConfirm");
            TutorialParam param = new TutorialParam("Tutorials.StageTitle", "Tutorials.StageText");
            // The stage will always follow the camera, so we set the mode to None
            TutorialManager.Instance.ShowTutorial(param, interactionTutorialTransform, () => stageConfirmed,
                mode: TutorialRemindMode.None);

            CurrState = AppState.PlaceStage;
        }

        yield return null;
    }

    /// <summary>
    /// When the stage is confirmed, 
    /// </summary>
    private IEnumerator ConfirmStage() {
        if (PlaneManager.Instance.UseImageTracking) {
            StageManager.Instance.CopyTransform();
            stageConfirmed = true;
        } else {
            StageManager.Instance.UpdateStageTransform();

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
        ArchetypeDisplayer displayer = ArchetypeManager.Instance.displayer;

        // Lock the buttons and show a loading text
        ControlPanelManager.Instance.DPanel.LockButtons(true);
        TutorialManager.Instance.ShowInstruction("Instructions.CalculateData");
        displayer.SetGreetingPose(true);

        // Connect to the API and retrieve the data
        displayer.Initialize();
        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            performer.Initialize();
        }

        NetworkError error = new NetworkError();

        while (error.status != NetworkStatus.Success) {
            yield return NetworkUtils.UserMatch(displayer.ArchetypeData, displayer.ArchetypeHealth, error);

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
            displayer.SetGreetingPose(false);
            StartCoroutine(ShowErrorInstruction(error));
            CurrState = AppState.Idle;
            showInfoCoroutine = null;
            yield break;
        }

        // We don't need the health data of the other two "profile/intervention"s right away, so we will do them
        // asynchronously.
        ArchetypeManager.Instance.SyncArchetype();

        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            if (performer.choice != HealthChoice.Custom) {
                StartCoroutine(performer.QueryHealth(new NetworkError(), null, true));
            } else {
                performer.ArchetypeHealth = new LongTermHealth(displayer.ArchetypeHealth);
                performer.DataReady = true;
            }
        }

        // Unlock the buttons and hide loading text
        ControlPanelManager.Instance.DPanel.LockButtons(false);
        TutorialManager.Instance.ClearInstruction();

        // Show the data on the panel
        DetailPanel panel = displayer.panel;
        panel.Toggle(true);
        panel.ToggleText(true);
        panel.BeginPulse();
        TimeProgressManager.Instance.Cycle(true);

        ArchetypeManager.Instance.displayer.SetGreetingPose(false);
        ArchetypeManager.Instance.LifestyleTutorial();
        CurrState = AppState.Idle;
        showInfoCoroutine = null;
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
        ControlPanelManager.Instance.Initialize();
        TimeProgressManager.Instance.TimeStop();
        StageManager.Instance.ResetVisualizations();
        ArchetypeManager.Instance.displayer.Reset();
        TutorialManager.Instance.Reset();

        if (showInfoCoroutine != null) {
            StopCoroutine(showInfoCoroutine);
            showInfoCoroutine = null;
        }

        CurrState = AppState.Idle;
    }

    /// <summary>
    /// Lets the user find another plane to place the stage.
    /// Will also reset the avatar because of tutorial placement issues.
    /// </summary>
    public void ResetStage() {
        if (CurrState != AppState.PlaceStage && CurrState != AppState.ChooseLanguage) {
            ResetAvatar();
        }

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
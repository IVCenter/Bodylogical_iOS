using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class MasterManager : MonoBehaviour {
    public static MasterManager Instance { get; private set; }

    private bool stageReady;
    private bool stageBuilt;

    public Text userNotification;

    public GamePhase CurrGamePhase { get; set; }

    public GameObject ParticleObj { get; set; }

    #region Unity Routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        CurrGamePhase = GamePhase.FindPlane;
        stageReady = false;
    }

    void Start() {
        ButtonSequenceManager.Instance.InitializeButtons();
        StartCoroutine(GameRunning());
    }
    #endregion

    #region Phases
    /// <summary>
    /// When the game is after PickArchetype: reset to PickArchetype
    /// When the game is in PickArchetype: reset to FindPlane
    /// </summary>
    public void ResetGame() {
        if (CurrGamePhase == GamePhase.PickArchetype) { // FindPlane
            stageReady = false;
            StageManager.Instance.DisableStage();
            StageManager.Instance.DisableControlPanel();
            HumanManager.Instance.StartSelectHuman = false;
            PlaneManager.Instance.RestartScan();

            CurrGamePhase = GamePhase.FindPlane;
        } else { // PickArchetype
            // Reset Activity, YearPanel and Prius
            StageManager.Instance.ResetVisualizations();
            TimeProgressManager.Instance.Reset();
            // In Year Panel the ribbon charts need to be destroyed.
            YearPanelManager.Instance.Reset();
            // Hide detail/choice panel
            DetailPanelManager.Instance.ToggleDetailPanel(false);
            ChoicePanelManager.Instance.ToggleChoicePanels(false);
            // Reset current archetype
            HumanManager.Instance.Reset();

            CurrGamePhase = GamePhase.PickArchetype;
        }

        // Common functions
        ButtonSequenceManager.Instance.InitializeButtons();
    }

    IEnumerator GameRunning() {
        while (true) {
            switch (CurrGamePhase) {
                case GamePhase.FindPlane:
                    yield return RunPhase1();
                    break;
                case GamePhase.PlaceStage:
                    yield return RunPhase2();
                    break;
                case GamePhase.PickArchetype:
                    yield return RunPhase3();
                    break;
                case GamePhase.ShowDetails:
                    yield return RunPhase4();
                    break;
                case GamePhase.Interaction:
                    yield return RunPhase5();
                    break;
                default:
                    yield return RunPhase5();
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// After finding a suitable plane, switch to phase 2.
    /// </summary>
    IEnumerator RunPhase1() {
        if (PlaneManager.Instance.PlaneFound) {
            CurrGamePhase = GamePhase.PlaceStage;
        }

        yield return null;
    }

    /// <summary>
    /// Prompts the user to confirm the stage.
    /// </summary>
    IEnumerator RunPhase2() {
        if (!stageReady) {
            userNotification.text = "Creating Stage...";
            yield return new WaitForSeconds(1.0f);

            if (ParticleObj != null) {
                ParticleObj.SetActive(false);
            }

            if (!stageBuilt) {
                stageBuilt = true; // only needs to build once
                StageManager.Instance.BuildStage();
            }
            stageReady = true;
        }

        StageManager.Instance.UpdateStageTransform();

        userNotification.text = "Double tap to confirm stage position";

        if ((Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2) || Input.GetKeyDown("space")) {
            userNotification.text = "";
            StageManager.Instance.SettleStage();
            PlaneManager.Instance.HideMainPlane();
            StageManager.Instance.EnableControlPanel();
            CurrGamePhase = GamePhase.PickArchetype;
        }

        yield return null;
    }

    /// <summary>
    /// The user needs to select a human model.
    /// When the human model is selected, put it into the center of the stage.
    /// Also, needs to set the model in the props view.
    /// </summary>
    IEnumerator RunPhase3() {
        if (!HumanManager.Instance.StartSelectHuman && !HumanManager.Instance.IsHumanSelected) {
            userNotification.text = "Select archetype to start\nPress \"Reset\" to relocate plane";
            HumanManager.Instance.StartSelectHuman = true;
        }

        if (HumanManager.Instance.IsHumanSelected) {
            userNotification.text = "";
            // move model to center
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            yield return new WaitForSeconds(0.5f);

            CurrGamePhase = GamePhase.ShowDetails;
        }

        yield return null;
    }

    /// <summary>
    /// The model stands out. Now showing the basic information
    /// </summary>
    IEnumerator RunPhase4() {
        userNotification.text = "Reading Archetype Info";
        yield return new WaitForSeconds(0.5f);
        HumanManager.Instance.ToggleUnselectedHuman(false);
        HumanManager.Instance.ToggleInteraction(false);
        userNotification.text = "";
        HumanManager.Instance.ExpandSelectedHumanInfo();
        YearPanelManager.Instance.LoadBounds(); // load data specific to the human body to the year panel
        YearPanelManager.Instance.LoadValues();
        TutorialText.Instance.Show("Please Select \"Predict\" Button", 6.0f);
        ButtonSequenceManager.Instance.SetPredictButton(true);
        CurrGamePhase = GamePhase.Interaction;

        yield return null;
    }

    /// <summary>
    /// Interaction will be triggered when certain buttons on the control panel is clicked.
    /// Nothing to be done here right now.
    /// </summary>
    IEnumerator RunPhase5() {
        yield return null;
    }
    #endregion
}

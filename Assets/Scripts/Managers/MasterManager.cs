using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour {
    public static MasterManager Instance { get; private set; }

    private bool stage_ready;

    public Text userNotification;

    public enum GamePhase {
        FindPlane,   // user is finding a suitable Plane Surface
        PlaceStage,   // user is placing the show stage
        PickArchetype,   // user starts to pick the the archetype;
        ShowDetails,   // expand information panel for the human in the center
        Interaction,   // user can do something with the control panel
    };

    public GamePhase CurrGamePhase { get; private set; }

    public GameObject ParticleObj { get; set; }

    #region Unity Routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        CurrGamePhase = GamePhase.FindPlane;
        stage_ready = false;
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
        // Common functions
        ButtonSequenceManager.Instance.InitializeButtons();

        if (CurrGamePhase == GamePhase.PickArchetype) { // FindPlane
            CurrGamePhase = GamePhase.FindPlane;
            stage_ready = false;
            StageManager.Instance.DisableStage();
            PlaneManager.Instance.RestartScan();
        } else { // PickArchetype
            CurrGamePhase = GamePhase.PickArchetype;
            // Reset Activity, YearPanel and Prius
            StageManager.Instance.ResetVisualizations();
            // In Year Panel the ribbon charts need to be destroyed.
            YearPanelManager.Instance.Reset();
            // In Prius the human might not be visible; need to enable selected human and hide all organs.
            HumanManager.Instance.SelectedHuman.SetActive(true);
            // Put the selected archetype back
            HumanManager.Instance.SelectedArchetype.SetHumanPosition();
            // Enable collider
            HumanManager.Instance.ToggleInteraction(true);
            // Reset current archetype, Show all archetype models
            HumanManager.Instance.ToggleUnselectedHuman(true);
            HumanManager.Instance.SelectedArchetype = null;
        }
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
        if (!stage_ready) {
            userNotification.text = "Creating Stage...";
            yield return new WaitForSeconds(1.0f);

            if (ParticleObj != null) {
                ParticleObj.SetActive(false);
            }

            StageManager.Instance.BuildStage();
            stage_ready = true;
        }

        StageManager.Instance.UpdateStageTransform();

        userNotification.text = "Double tap to confirm stage position";

        if ((Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2) || Input.GetKeyDown("space")) {
            userNotification.text = "";
            StageManager.Instance.SettleStage();
            PlaneManager.Instance.HideMainPlane();

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
            HumanManager.Instance.StartSelectHuman = true;
            TutorialText.Instance.ShowDouble("Please select an archetype to start",
                "Click \"Reset\" to move the stage", 5.0f);
            StageManager.Instance.EnableControlPanel();
        }

        if (HumanManager.Instance.IsHumanSelected) {
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

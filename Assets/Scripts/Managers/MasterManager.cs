using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour {

    public static MasterManager Instance;

    private bool stage_ready;

    public Text userNotification;

    public enum GamePhase {
        Phase1,   // user is finding a suitable Plane Surface
        Phase2,   // user is placing the show stage
        Phase3,   // user starts to pick the the archetype;
        Phase4,   // expand information panel for the human in the center
        Phase5,   // user can do something with the control panel
        Phase6
    };

    public GamePhase CurrGamePhase {
        get; private set;
    }

    public GameObject ParticleObj {
        get; set;
    }

    #region ControlMethods
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        CurrGamePhase = GamePhase.Phase1;
        stage_ready = false;
    }

    // Use this for initialization
    void Start() {
        ButtonSequenceManager.Instance.InitializeButtonsActive();
        StartCoroutine(GameRunning());
    }

    public void ResetGame() {
        CurrGamePhase = GamePhase.Phase1;
        stage_ready = false;
        StageManager.Instance.DisableStage();
        PlaneManager.Instance.RestartScan();

    }
    #endregion

    #region Phases
    IEnumerator GameRunning() {
        while (true) {
            switch (CurrGamePhase) {
                case GamePhase.Phase1:
                    yield return RunPhase1();
                    break;
                case GamePhase.Phase2:
                    yield return RunPhase2();
                    break;
                case GamePhase.Phase3:
                    yield return RunPhase3();
                    break;
                case GamePhase.Phase4:
                    yield return RunPhase4();
                    break;
                case GamePhase.Phase5:
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
            CurrGamePhase = GamePhase.Phase2;
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

        if ((Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2) || Input.GetKeyDown(KeyCode.N)) {
            userNotification.text = "";
            StageManager.Instance.SettleStage();
            PlaneManager.Instance.HideMainPlane();

            CurrGamePhase = GamePhase.Phase3;
        }

        yield return null;
    }

    /// <summary>
    /// The user needs to select a human model.
    /// </summary>
    IEnumerator RunPhase3() {
        if (!HumanManager.Instance.startSelectHuman && !HumanManager.Instance.IsHumanSelected) {
            HumanManager.Instance.startSelectHuman = true;
            userNotification.text = "Please select an archetype to start";
        }

        if (HumanManager.Instance.IsHumanSelected) {
            userNotification.text = "";
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            yield return new WaitForSeconds(0.5f);

            YearPanelManager.Instance.GoAndRequestPanelInfo();

            CurrGamePhase = GamePhase.Phase4;
        }

        yield return null;
    }

    /// <summary>
    /// The model stands out. Now showing the basic information
    /// </summary>
    IEnumerator RunPhase4() {
        userNotification.text = "Reading Archetype Info";
        yield return new WaitForSeconds(0.5f);
        HumanManager.Instance.HideUnselectedHuman();
        userNotification.text = "";
        HumanManager.Instance.IfExpandSelectedHumanInfo(true);
        StageManager.Instance.EnableControlPanel();
        TutorialText.Instance.Show("Please Select \"Predict\" Button", 6.0f);

        CurrGamePhase = GamePhase.Phase5;

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

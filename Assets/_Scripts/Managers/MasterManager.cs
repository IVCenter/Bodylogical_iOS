using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour {

    public static MasterManager Instance;

    private bool stage_ready;
    private GameObject particle_obj;
    private int curr_year;

    public Text userNotification;

    public enum GamePhase
    {
        Phase1,   // user is finding a suitable Plane Surface
        Phase2,   // user is placing the show stage
        Phase3,   // user starts to pick the the archetype;
        Phase4,   // expand information panel for the human in the center
        Phase5,    // the user can do something with the control panel
        Phase6
    };

    private GamePhase gamePhase;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        gamePhase = GamePhase.Phase1;
        stage_ready = false;
        curr_year = 2018;
    }

    // Use this for initialization
    void Start () {

        StartCoroutine(GameRunning());
	}
	
	// Update is called once per frame
	void Update () {


	}


    public void GoToNextPeriod(int choice){

        HumanManager.Instance.IfExpandSelectedHumanInfo(false);

        curr_year += 1;

        HumanManager.Instance.SetHumanCurrentYear(curr_year);

        if (choice == 0)
        {

        }
        else if (choice == 1)
        {

        }
        else if (choice == 2)
        {

        }
    }



    IEnumerator GameRunning(){

        while(true){
            if (gamePhase == GamePhase.Phase1)
            {
                yield return RunPhase1();
            }

            if (gamePhase == GamePhase.Phase2)
            {
                yield return RunPhase2();
            }

            if (gamePhase == GamePhase.Phase3){

                yield return RunPhase3();
            }

            if (gamePhase == GamePhase.Phase4) {

                yield return RunPhase4();
            }

            if (gamePhase == GamePhase.Phase5)
            {

                yield return RunPhase5();
            }

            yield return null;
        }

    }

    public void ResetGame(){
        gamePhase = GamePhase.Phase1;
        stage_ready = false;
        StageManager.Instance.DisableStage();
        PlaneManager.Instance.RestartScan();
    
    }

    public GamePhase GetCurrentGamePhase(){
        return gamePhase;
    }

    public void SetParticleObject(GameObject particle_prefab){
        particle_obj = particle_prefab;
    }

    IEnumerator RunPhase1(){
        if (PlaneManager.Instance.IsPlaneFound())
        {
            gamePhase = GamePhase.Phase2;
        }

        yield return null;
    }



    IEnumerator RunPhase2(){
    

        if (!stage_ready)
        {
            userNotification.text = "Creating Stage...";
            yield return new WaitForSeconds(1.0f);

            if (particle_obj != null)
            {
                particle_obj.SetActive(false);
            }

            StageManager.Instance.BuildStage();
            stage_ready = true;
        }

        StageManager.Instance.UpdateStageTransform();

        userNotification.text = "Double tap to confirm stage position";

        if ((Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2) || Input.GetKeyDown(KeyCode.N))
        {
            userNotification.text = "";
            StageManager.Instance.SettleStage();
            PlaneManager.Instance.HideMainPlane();

            gamePhase = GamePhase.Phase3;
        }

        yield return null;
    }

    IEnumerator RunPhase3(){

        if(!HumanManager.Instance.startSelectHuman && !HumanManager.Instance.IsAHumanSelected()){
            HumanManager.Instance.startSelectHuman = true;
            userNotification.text = "Please select an archetype to start";
        }

        if (HumanManager.Instance.IsAHumanSelected()){
            //HumanManager.Instance.MoveSelectedHumanToCenter();
            userNotification.text = "";
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            yield return new WaitForSeconds(0.5f);
            gamePhase = GamePhase.Phase4;

        }

        yield return null;
    }

    IEnumerator RunPhase4(){

        userNotification.text = "Reading Archetype Info";
        yield return new WaitForSeconds(0.5f);
        HumanManager.Instance.HideUnselectedHuman();
        userNotification.text = "";
        HumanManager.Instance.IfExpandSelectedHumanInfo(true);
        StageManager.Instance.EnableControlPanel();

        gamePhase = GamePhase.Phase5;

        yield return null;
    }

    IEnumerator RunPhase5(){



        yield return null;
    }
}

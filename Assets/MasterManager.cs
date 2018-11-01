using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour {

    private bool stage_ready;

    public Text userNotification;

    public enum GamePhase
    {
        Phase1,  // user is finding a suitable Plane Surface
        Phase2,  // user is placing the show stage
        Phase3   // game started.
    };

    private GamePhase gamePhase;

	// Use this for initialization
	void Start () {
        gamePhase = GamePhase.Phase1;
        stage_ready = false;

        StartCoroutine(GameRunning());
	}
	
	// Update is called once per frame
	void Update () {


	}



    IEnumerator GameRunning(){

        while(true){
            if (gamePhase == GamePhase.Phase1)
            {
                if (PlaneManager.Instance.IsPlaneFound())
                {
                    gamePhase = GamePhase.Phase2;
                }
            }

            if (gamePhase == GamePhase.Phase2)
            {
                if (!stage_ready)
                {
                    userNotification.text = "Creating Stage...";
                    yield return new WaitForSeconds(2.0f);

                    StageManager.Instance.BuildStage();
                    stage_ready = true;
                }
                StageManager.Instance.UpdateStageTransform();
                userNotification.text = "Double tap to confirm stage position";

                if (Input.touchCount > 0 && Input.GetTouch(0).tapCount >= 2)
                {
                    userNotification.text = "";
                    gamePhase = GamePhase.Phase3;
                }
            }


            if (Input.GetKeyDown(KeyCode.N))
            {
                gamePhase = GamePhase.Phase2;
            }
            yield return null;
        }


    }
}

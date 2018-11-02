using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collections.Hybrid.Generic;

public class StageManager : MonoBehaviour {

    public static StageManager Instance;
    public GameObject stage;

    public Transform[] positionList;

    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }
    }

    public Transform GetAvailablePosInWorld(){

        foreach (Transform trans in positionList){
            if (posAvailableMap[trans]){
                posAvailableMap[trans] = false;
                return trans;
            }
        }
        return null;
    }

    public void BuildStage(){
        for (int i = 0; i < positionList.Length; i++){
            string profile_name = "Profile" + i;
            HumanManager.Instance.CreateArchitype(profile_name);
        }
    }

    public void UpdateStageTransform(){

        if (!stage.activeSelf){
            stage.SetActive(true);
        }

        if (!stage.GetComponent<MeshRenderer>().enabled){
            stage.GetComponent<MeshRenderer>().enabled = true;
        }

        if (CursorManager.Instance.cursor.GetCurrentFocusedObj() != null){

            GameObject obj = CursorManager.Instance.cursor.GetCurrentFocusedObj();

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>()!=null){
                Vector3 cursorPos = CursorManager.Instance.cursor.GetCursorPosition();
                Vector3 stageCenter = stage.transform.GetChild(0).position;
                Vector3 diff = stage.transform.position - stageCenter;
                stage.transform.position = cursorPos + diff;
                AdjustStageRotation(PlaneManager.Instance.GetMainPlane());
            }
        }

        Color lerpedColor = Color.Lerp(Color.white, futureBlue, Mathf.PingPong(Time.time, 1));

        stage.GetComponent<MeshRenderer>().material.color = lerpedColor;
    }

    public void DisableStage(){
        stage.SetActive(false);
    }

    public void SettleStage(){
        stage.GetComponent<MeshRenderer>().enabled = false;
    }

    // Use this for initialization
    void Start () {

        posAvailableMap = new LinkedListDictionary<Transform, bool>();

        foreach (Transform trans in positionList){
            posAvailableMap.Add(trans, true);
        }

        futureBlue = new Color(66 / 255.0f, 220 / 255.0f, 255 / 255.0f);

        DisableStage();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void AdjustStageRotation(GameObject plane){
        stage.transform.rotation = plane.transform.rotation;

        while(Vector3.Dot((stage.transform.position-Camera.main.transform.position), stage.transform.forward) < 0){
            stage.transform.Rotate(0, 90, 0);
        }
        
    }
}

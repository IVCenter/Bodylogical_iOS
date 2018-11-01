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

        if (CursorManager.Instance.cursor.GetCurrentFocusedObj() != null){

            GameObject obj = CursorManager.Instance.cursor.GetCurrentFocusedObj();

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>()!=null){
                Vector3 cursorPos = CursorManager.Instance.cursor.GetCursorPosition();
                Vector3 stageCenter = stage.transform.GetChild(0).position;
                Vector3 diff = stage.transform.position - stageCenter;
                stage.transform.position = cursorPos + diff;
                stage.transform.rotation = Quaternion.LookRotation(stage.transform.position);
                stage.transform.rotation = Quaternion.Euler(0, stage.transform.rotation.eulerAngles.y, 0);
            }
        }
    }


    // Use this for initialization
    void Start () {

        posAvailableMap = new LinkedListDictionary<Transform, bool>();

        foreach (Transform trans in positionList){
            posAvailableMap.Add(trans, true);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

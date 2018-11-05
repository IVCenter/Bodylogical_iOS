using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collections.Hybrid.Generic;

public class HumanManager : MonoBehaviour {

    public static HumanManager Instance;
    public bool startSelectHuman;

    public GameObject male_prefab;
    public GameObject female_prefab;

    //public GameObject humanModel;

    private GameObject selected_human;
    private bool isAHumanSelected;

    public LinkedListDictionary<string, Archetype> archetypeMap;

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }
    }

    private bool CheckHumanSelection(){

        // DebugText.Instance.Log("Checking Human Selection...");
        foreach (Archetype human in archetypeMap.Values){
            if (human.GetHumanObject().GetComponentInChildren<HumanInteract>().isSelected){
                selected_human = human.GetHumanObject();
                return true;
            }
        }

        return false;
    }

    public bool MoveSelectedHumanToCenter(){

        if(!isAHumanSelected){
            return false;
        }

        StartCoroutine(MoveHumanTowardCenter());

        return true;
    }

    public bool CreateArchitype(string ProfileName, string Name = "Bob", float Weight = 160, string sex = "male"){

        Archetype go = new Archetype(ProfileName, Name, Weight, sex);
        go.InstantiateModel(male_prefab, female_prefab);

        if(!go.CreateModel(sex)){
            return false;
        }

        archetypeMap.Add(ProfileName, go);
        return true;
    }

    public bool IsAHumanSelected(){
        return isAHumanSelected;
    }


    public void IfExpandSelectedHumanInfo(bool expand){
        if (selected_human == null){
            return;
        }

        DebugText.Instance.Log("IfExpand is called!!!!!!!");

        selected_human.transform.Search("Canvas").gameObject.SetActive(!expand);
        selected_human.transform.Search("Panels").gameObject.SetActive(expand);
    }

    public void ResetManager(){

        isAHumanSelected = false;
        selected_human = null;
        foreach (Archetype human in archetypeMap.Values){
            human.Clear();
        }
        archetypeMap.Clear();
    }



    // Use this for initialization
    void Start () {
        archetypeMap = new LinkedListDictionary<string, Archetype>();
        isAHumanSelected = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(!isAHumanSelected){
            if(startSelectHuman && CheckHumanSelection()){
                isAHumanSelected = true;
                startSelectHuman = false;
            }
        }
	}

    IEnumerator MoveHumanTowardCenter(){

        if (isAHumanSelected && selected_human != null)
        {
            float moveddist = 0;

            Vector3 startpos = selected_human.transform.position;
            Vector3 endpos = StageManager.Instance.stage.transform.position;

            float journey_length = Vector3.Distance(startpos, endpos);

            while (moveddist < journey_length)
            {
                float fracJourney = moveddist / journey_length;
                selected_human.transform.position = Vector3.Lerp(startpos, endpos, fracJourney);
                moveddist += Time.deltaTime;
                yield return null;
            }

            selected_human.transform.position = endpos;
            selected_human.transform.GetChild(1).transform.rotation = StageManager.Instance.stage.transform.rotation;
            selected_human.transform.GetChild(1).transform.Rotate(0, 180, 0);
        }

        yield return null;
    }
}

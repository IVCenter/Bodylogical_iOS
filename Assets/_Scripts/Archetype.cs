using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archetype : MonoBehaviour {


    public GameObject male_prefab;
    public GameObject female_prefab;

    private GameObject model;
    public string profile_name;
    public string model_name;
    public float weight;
    public string sex;
    public bool wasted;

    public Archetype(string p_name, string m_name, float w_weight, string s_sex){

        profile_name = p_name;
        model_name = m_name;
        weight = w_weight;
        sex = s_sex;
        wasted = false;

    }

    public void InstantiateModel(GameObject male, GameObject female){
        male_prefab = male;
        female_prefab = female;
    }

    // return if model creation was successful
    public bool CreateModel(string sex){

        if (sex == "male"){
            // create Male model
            model = GameObject.Instantiate(male_prefab);
        }
        else if (sex == "female"){
            model = GameObject.Instantiate(female_prefab);
        }

        Transform trans = StageManager.Instance.GetAvailablePosInWorld();

        if(trans == null){
            return false;
        }

        model.transform.parent = StageManager.Instance.stage.transform;
        Vector3 footPoint = model.transform.GetChild(0).transform.position;
        Vector3 diff = model.transform.position - footPoint;
        model.transform.position = trans.position + diff;
        model.transform.GetChild(1).rotation = trans.rotation;

        return true;
    }

    public void EnableInformationPanel(){
        DebugText.Instance.Log("Panel Enabled");
    }

    public void DisableInformationPanel()
    {
        DebugText.Instance.Log("Panel Disabled");
    }


    public GameObject GetHumanObject(){
        return model;
    }

    public void Clear(){
        Destroy(model);
        wasted = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archetype : MonoBehaviour {


    public GameObject male_prefab;
    public GameObject female_prefab;

    private GameObject model;
    public string profile_name;
    public string model_name;
    public float weight;
    public string sex;
    public string health_condition;
    public bool wasted;

    public Archetype(string p_name, string m_name, float w_weight, string s_sex, string health_cond){

        profile_name = p_name;
        model_name = m_name;
        weight = w_weight;
        sex = s_sex;
        health_condition = health_cond;
        wasted = false;

    }

    public void InstantiateModel(GameObject male, GameObject female){
        male_prefab = male;
        female_prefab = female;
    }

    // return if model creation was successful
    public bool CreateModel(string sex){

        // try get an avaliable position
        Transform trans = StageManager.Instance.GetAvailablePosInWorld();

        if(trans == null){
            return false;
        }

        if (sex == "male")
        {
            // create Male model
            model = GameObject.Instantiate(male_prefab);
        }
        else if (sex == "female")
        {
            model = GameObject.Instantiate(female_prefab);
        }

        // set model poses
        model.transform.parent = StageManager.Instance.stage.transform;
        Vector3 footPoint = model.transform.GetChild(0).transform.position;
        Vector3 diff = model.transform.position - footPoint;
        model.transform.position = trans.position + diff;
        model.transform.GetChild(1).rotation = trans.rotation;

        // set model information
        model.transform.Search("Title").GetComponent<Text>().text = profile_name;
        model.transform.Search("Name").GetComponent<Text>().text = "Name: " + model_name;
        model.transform.Search("Disease").GetComponent<Text>().text = "Cardio Health: " + health_condition;
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

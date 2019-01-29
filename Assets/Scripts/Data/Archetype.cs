using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archetype : MonoBehaviour {
    public GameObject HumanObject {
        get; private set;
    }

    public string profile_name;
    public string model_name;
    public float weight;
    public string sex;
    public string health_condition;
    public GameObject modelPrefab;


    public Archetype(string p_name, string m_name, float w_weight, string s_sex, string health_cond) {
        profile_name = p_name;
        model_name = m_name;
        weight = w_weight;
        sex = s_sex;
        health_condition = health_cond;
    }

    /// <summary>
    /// Creates the model.
    /// </summary>
    /// <returns><c>true</c>, if model was created, <c>false</c> otherwise.</returns>
    public bool CreateModel() {
        // try get an avaliable position
        Transform trans = StageManager.Instance.GetAvailablePosInWorld();

        if (trans == null) {
            return false;
        }

        GameObject model = Instantiate(modelPrefab);
        HumanObject = Instantiate(ArchetypeContainer.Instance.modelTemplate);

        if (HumanObject == null) {
            TutorialText.Instance.Show("Human is null", 5);
        }

        // Set model parent hierarchy
        Transform modelTransform = HumanObject.transform.Find("ModelParent/model");
        if (modelTransform == null) {
            TutorialText.Instance.Show("modelTransform is null", 5);
        }
        model.transform.SetParent(modelTransform, false);

        // set model poses
        HumanObject.transform.parent = StageManager.Instance.stage.transform;
        Vector3 footPoint = HumanObject.transform.GetChild(0).position;
        Vector3 diff = HumanObject.transform.position - footPoint;
        HumanObject.transform.position = trans.position + diff;
        HumanObject.transform.GetChild(1).rotation = trans.rotation;

        // set model information
        HumanObject.transform.Search("Occupation").GetComponent<Text>().text = profile_name;
        HumanObject.transform.Search("Name").GetComponent<Text>().text = "Name: " + model_name;
        HumanObject.transform.Search("Disease").GetComponent<Text>().text = "Health: " + health_condition;

        return true;
    }

    public void EnableInformationPanel() {
        DebugText.Instance.Log("Panel Enabled");
    }

    public void DisableInformationPanel() {
        DebugText.Instance.Log("Panel Disabled");
    }

    public void Clear() {
        Destroy(HumanObject);
    }
}

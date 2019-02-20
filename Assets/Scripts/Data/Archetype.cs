using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archetype : MonoBehaviour {
    public GameObject HumanObject { get; private set; }

    public string modelName;
    public string occupation;
    public string sex;
    public HealthStatus healthCondition;

    public GameObject modelPrefab;
    public Lifestyle ModelLifestyle { get { return GetComponent<Lifestyle>(); } }
    public Transform StageTrans { get; private set; }

    public Archetype(string p_name, string m_name, string s_sex, HealthStatus health_cond) {
        occupation = p_name;
        modelName = m_name;
        sex = s_sex;
        healthCondition = health_cond;
    }

    /// <summary>
    /// Creates the model.
    /// </summary>
    /// <returns><c>true</c>, if model was created, <c>false</c> otherwise.</returns>
    public bool CreateModel() {
        // try get an avaliable position
        StageTrans = StageManager.Instance.GetAvailablePosInWorld();

        if (StageTrans == null) {
            return false;
        }

        GameObject model = Instantiate(modelPrefab);
        HumanObject = Instantiate(ArchetypeContainer.Instance.modelTemplate);

        if (HumanObject == null) {
            TutorialText.Instance.Show("Human is null", 5);
        }

        // Set model parent hierarchy
        Transform modelTransform = HumanObject.transform.Find("model");
        if (modelTransform == null) {
            TutorialText.Instance.Show("modelTransform is null", 5);
        }
        model.transform.SetParent(modelTransform, false);

        // set model poses
        HumanObject.transform.parent = StageManager.Instance.stage.transform;
        SetHumanPosition();

        // set model information
        HumanObject.transform.Search("Occupation").GetComponent<Text>().text = occupation;
        HumanObject.transform.Search("Name").GetComponent<Text>().text = "Name: " + modelName;
        HumanObject.transform.Search("Disease").GetComponent<Text>().text = "Health: " + healthCondition;

        return true;
    }

    public void SetHumanPosition() {
        Vector3 footPoint = HumanObject.transform.GetChild(0).position;
        Vector3 diff = HumanObject.transform.position - footPoint;
        HumanObject.transform.position = StageTrans.position + diff;
        HumanObject.transform.rotation = StageTrans.rotation;
    }

    public void Clear() {
        Destroy(HumanObject);
        HumanObject = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archetype : MonoBehaviour {
    public GameObject HumanObject { get; private set; }

    public string modelName;
    public string occupation;
    public Gender sex;
    public HealthStatus healthCondition;

    public GameObject modelPrefab;
    public Lifestyle ModelLifestyle { get { return GetComponent<Lifestyle>(); } }
    public Transform StageTrans { get; private set; }

    private static readonly Dictionary<HealthStatus, string> statusKeyDictionary = new Dictionary<HealthStatus, string> {
        {HealthStatus.Good, "General.StatusGood"},
        {HealthStatus.Intermediate, "General.StatusIntermediate"},
        {HealthStatus.Bad, "General.StatusBad"}
    };

    public Archetype(string p_name, string m_name, Gender s_sex, HealthStatus health_cond) {
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
            return false;
        }

        // Set model parent hierarchy
        Transform modelTransform = HumanObject.transform.Find("model");
        if (modelTransform == null) {
            return false;
        }
        model.transform.SetParent(modelTransform, false);

        // set model poses
        HumanObject.transform.parent = StageManager.Instance.stage.transform;
        SetHumanPosition();

        // set model information
        HumanObject.transform.Search("Name").GetComponent<LocalizedText>().SetText("Archetypes.Name", new LocalizedParam(modelName, true));
        HumanObject.transform.Search("Occupation").GetComponent<LocalizedText>().SetText("Archetypes.Occupation", new LocalizedParam(occupation, true));
        HumanObject.transform.Search("Disease").GetComponent<LocalizedText>().SetText("Archetypes.Status", new LocalizedParam(statusKeyDictionary[healthCondition], true));

        return true;
    }

    public void SetHumanPosition() {
        Vector3 footPoint = HumanObject.transform.GetChild(0).position;
        Vector3 diff = HumanObject.transform.position - footPoint;
        HumanObject.transform.position = StageTrans.position + diff;
        HumanObject.transform.rotation = StageTrans.rotation;
    }
}

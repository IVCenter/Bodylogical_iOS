using System.Collections.Generic;
using UnityEngine;

public class Archetype {
    public GameObject HumanObject { get; private set; }
    public int id;
    public Gender gender;
    public int age;
    public HealthStatus status;
    public string modelString;

    public Dictionary<HealthChoice, Lifestyle> lifestyleDict;
    public Transform StageTrans { get; private set; }

    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Name { get { return string.Format("Archetypes.P{0}Name", id); } }
    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Occupation { get { return string.Format("Archetypes.P{0}Occupation", id); } }

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

        GameObject model = Object.Instantiate(Resources.Load<GameObject>(string.Format("Prefabs/{0}", modelString)));
        HumanObject = Object.Instantiate(ArchetypeContainer.Instance.modelTemplate);

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
        HumanObject.transform.parent = StageManager.Instance.characterParent;
        SetHumanPosition();

        // set model information
        HumanObject.transform.Search("Name").GetComponent<LocalizedText>().
            SetText("Archetypes.Name", new LocalizedParam(Name, true));
        HumanObject.transform.Search("Age").GetComponent<LocalizedText>().
            SetText("Archetypes.Age", new LocalizedParam(age));
        HumanObject.transform.Search("Occupation").GetComponent<LocalizedText>().
            SetText("Archetypes.Occupation", new LocalizedParam(Occupation, true));
        HumanObject.transform.Search("Disease").GetComponent<LocalizedText>().
            SetText("Archetypes.Status", new LocalizedParam(LocalizationDicts.statuses[status], true));

        return true;
    }

    public void SetHumanPosition() {
        Vector3 footPoint = HumanObject.transform.GetChild(0).position;
        Vector3 diff = HumanObject.transform.position - footPoint;
        HumanObject.transform.position = StageTrans.position + diff;
        HumanObject.transform.rotation = StageTrans.rotation;
    }
}

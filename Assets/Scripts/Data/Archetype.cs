using System.Collections.Generic;
using UnityEngine;

public class Archetype {
    public GameObject Model { get; private set; }
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
    public string Name => string.Format("Archetypes.P{0}Name", id);
    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Occupation => string.Format("Archetypes.P{0}Occupation", id);

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
        Model = Object.Instantiate(ArchetypeLoader.Instance.modelTemplate);

        if (Model == null) {
            return false;
        }

        // Set model parent hierarchy
        Transform modelTransform = Model.transform.Find("model");
        if (modelTransform == null) {
            return false;
        }
        model.transform.SetParent(modelTransform, false);

        // set model poses
        Model.transform.parent = StageManager.Instance.characterParent;
        SetModelPosition();

        // set model information
        Model.transform.Search("Name").GetComponent<LocalizedText>().
            SetText("Archetypes.Name", new LocalizedParam(Name, true));
        Model.transform.Search("Age").GetComponent<LocalizedText>().
            SetText("Archetypes.Age", new LocalizedParam(age));
        Model.transform.Search("Occupation").GetComponent<LocalizedText>().
            SetText("Archetypes.Occupation", new LocalizedParam(Occupation, true));
        Model.transform.Search("Disease").GetComponent<LocalizedText>().
            SetText("Archetypes.Status", new LocalizedParam(LocalizationDicts.statuses[status], true));

        return true;
    }

    public void SetModelPosition() {
        Vector3 footPoint = Model.transform.GetChild(0).position;
        Vector3 diff = Model.transform.position - footPoint;
        Model.transform.position = StageTrans.position + diff;
        Model.transform.rotation = StageTrans.rotation;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Archetype {
    public GameObject Model { get; private set; }
    public Material Mat { get; private set; }
    public int id;
    public Gender gender;
    public int age;
    public HealthStatus status;
    public string modelString;

    public Dictionary<HealthChoice, Lifestyle> lifestyleDict;

    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Name => string.Format("Archetypes.P{0}Name", id);
    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Occupation => string.Format("Archetypes.P{0}Occupation", id);

    private Transform stageTransform;

    /// <summary>
    /// Creates the model.
    /// </summary>
    /// <returns><c>true</c>, if model was created, <c>false</c> otherwise.</returns>
    public void CreateModel() {
        GameObject figure = Object.Instantiate(Resources.Load<GameObject>(string.Format("Prefabs/{0}", modelString)));
        Mat = figure.transform.GetChild(0).GetComponent<Renderer>().material;
        Model = Object.Instantiate(ArchetypeLoader.Instance.modelTemplate);

        // Set model parent hierarchy
        Transform modelTransform = Model.transform.Find("model");
        figure.transform.SetParent(modelTransform, false);

        // Set archetype info canvas
        Model.transform.Search("Name").GetComponent<LocalizedText>().
            SetText("Archetypes.Name", new LocalizedParam(Name, true));
        Model.transform.Search("Age").GetComponent<LocalizedText>().
            SetText("Archetypes.Age", new LocalizedParam(age));
        Model.transform.Search("Occupation").GetComponent<LocalizedText>().
            SetText("Archetypes.Occupation", new LocalizedParam(Occupation, true));
        Model.transform.Search("Disease").GetComponent<LocalizedText>().
            SetText("Archetypes.Status", new LocalizedParam(LocalizationDicts.statuses[status], true));
    }

    public void SetModelPosition(Transform stageTransform = null) {
        if (stageTransform != null) {
            this.stageTransform = stageTransform;
        }

        Vector3 footPoint = Model.transform.GetChild(0).position;
        Vector3 diff = Model.transform.position - footPoint;
        Model.transform.position = this.stageTransform.position + diff;
        Model.transform.rotation = this.stageTransform.rotation;
    }
}

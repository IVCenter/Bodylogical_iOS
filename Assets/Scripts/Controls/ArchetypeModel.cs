using UnityEngine;

/// <summary>
/// A wrapper class that contains an archetype data object and its model.
/// </summary>
public class ArchetypeModel {
    public Archetype archetype;
    public GameObject model;
    public Material mat;
    public Animator animator;
    public GameObject infoCanvas;
    public HeartIndicator heart;

    public ArchetypeModel(Archetype archetype) {
        this.archetype = archetype;

        GameObject figure = Object.Instantiate(Resources.Load<GameObject>(string.Format("Prefabs/{0}", archetype.modelString)));
        model = Object.Instantiate(ArchetypeLoader.Instance.modelTemplate);
        mat = figure.transform.GetChild(0).GetComponent<Renderer>().material;
        animator = figure.transform.GetComponent<Animator>();

        // Set model parent hierarchy
        Transform modelTransform = model.transform.Find("model");
        figure.transform.SetParent(modelTransform, false);

        // Set archetype info canvas
        infoCanvas = model.transform.Search("BasicInfoCanvas").gameObject;
        infoCanvas.transform.Search("Name").GetComponent<LocalizedText>().
            SetText("Archetypes.Name", new LocalizedParam(archetype.Name, true));
        infoCanvas.transform.Search("Age").GetComponent<LocalizedText>().
            SetText("Archetypes.Age", new LocalizedParam(archetype.age));
        infoCanvas.transform.Search("Occupation").GetComponent<LocalizedText>().
            SetText("Archetypes.Occupation", new LocalizedParam(archetype.Occupation, true));
        infoCanvas.transform.Search("Disease").GetComponent<LocalizedText>().
            SetText("Archetypes.Status", new LocalizedParam(LocalizationDicts.statuses[archetype.status], true));

        heart = model.transform.Search("Health Indicator").GetComponent<HeartIndicator>();
    }

    public void Dispose() {
        Object.Destroy(model);
    }
}

using UnityEngine;

/// <summary>
/// A wrapper class that contains an archetype data object and its model.
/// </summary>
public class ArchetypeModel {
    public Archetype ArchetypeData { get; }
    public GameObject Model { get; }
    public Material Mat { get; }
    public Animator ArchetypeAnimator { get; }
    public HeartIndicator Heart { get; }
    public SwitchIcon Icon { get; }
    
    public Lifestyle ArchetypeLifestyle { get; }
    public LongTermHealth ArchetypeHealth { get; }

    public ArchetypeModel(Archetype archetypeData, Transform parent, Lifestyle lifestyle, LongTermHealth health) {
        ArchetypeData = archetypeData;
        ArchetypeLifestyle = lifestyle;
        ArchetypeHealth = health;

        Model = Object.Instantiate(ArchetypeManager.Instance.modelPrefab, parent, false);
        Transform modelTransform = Model.transform.Find("model");
        
        GameObject figure = Object.Instantiate(Resources.Load<GameObject>($"Prefabs/{archetypeData.modelString}"),
            modelTransform, false);
        Mat = figure.transform.GetChild(0).GetComponent<Renderer>().material;
        ArchetypeAnimator = figure.transform.GetComponent<Animator>();
        
        Heart = Model.transform.Search("Health Indicator").GetComponent<HeartIndicator>();
        Icon = Model.transform.Search("Button Canvas").GetComponent<SwitchIcon>();
    }

    public void Dispose() {
        Object.Destroy(Model);
    }
}

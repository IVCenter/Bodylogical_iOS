using UnityEngine;

/// <summary>
/// A wrapper class for the models in the visualizations.
/// </summary>
public class ArchetypePerformer : ArchetypeModel {
    public HeartIndicator Heart { get; }
    public SwitchIcon Icon { get; }

    public Lifestyle ArchetypeLifestyle { get; }
    public LongTermHealth ArchetypeHealth { get; }

    public ArchetypePerformer(Archetype archetypeData, Transform parent, Lifestyle lifestyle, LongTermHealth health)
        : base(archetypeData, parent) {
        ArchetypeData = archetypeData;
        ArchetypeLifestyle = lifestyle;
        ArchetypeHealth = health;

        Model = Object.Instantiate(ArchetypeManager.Instance.performerPrefab, parent, false);
        Transform modelTransform = Model.transform.Find("model");

        GameObject figure = Object.Instantiate(Resources.Load<GameObject>($"Prefabs/{archetypeData.modelString}"),
            modelTransform, false);
        Mat = figure.transform.GetChild(0).GetComponent<Renderer>().material;
        ArchetypeAnimator = figure.transform.GetComponent<Animator>();

        Heart = Model.transform.Search("Health Indicator").GetComponent<HeartIndicator>();
        Icon = Model.transform.Search("Button Canvas").GetComponent<SwitchIcon>();
    }
}
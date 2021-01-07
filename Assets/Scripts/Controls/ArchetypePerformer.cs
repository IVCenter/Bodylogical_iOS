using UnityEngine;

/// <summary>
/// A wrapper class for the models in the visualizations.
/// </summary>
public class ArchetypePerformer : ArchetypeModel {
    public HealthChoice Choice { get; }
    public Lifestyle ArchetypeLifestyle { get; }
    public LongTermHealth ArchetypeHealth { get; }
    
    public SwitchIcon Icon { get; }
    public ActivityController Activity { get; }
    public PriusController Prius { get; }
    public StatsController Stats { get; }

    public ArchetypePerformer(Archetype archetypeData, Transform parent, HealthChoice choice, Lifestyle lifestyle,
        LongTermHealth health, BackwardsProps props)
        : base(ArchetypeManager.Instance.performerPrefab, archetypeData, parent) {
        Choice = choice;
        ArchetypeLifestyle = lifestyle;
        ArchetypeHealth = health;

       Icon = Model.GetComponentInChildren<SwitchIcon>();
       Activity = Model.GetComponentInChildren<ActivityController>();
       Activity.Initialize(this, props);
       Prius = Model.GetComponentInChildren<PriusController>();
       Stats = Model.GetComponentInChildren<StatsController>();
    }
}
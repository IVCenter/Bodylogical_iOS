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
    public Visualization CurrentVisualization { get; private set; } = Visualization.Activity;

    public ArchetypePerformer(Archetype archetypeData, Transform parent, HealthChoice choice, Lifestyle lifestyle,
        LongTermHealth health, BackwardsProps props)
        : base(ArchetypeManager.Instance.performerPrefab, archetypeData, parent) {
        Choice = choice;
        ArchetypeLifestyle = lifestyle;
        ArchetypeHealth = health;

        Icon = Model.GetComponentInChildren<SwitchIcon>();
        Icon.Initialize(this);
        Activity = Model.GetComponentInChildren<ActivityController>(true);
        Activity.Initialize(this, props);
        Prius = Model.GetComponentInChildren<PriusController>(true);
        Prius.Initialize(this);
        Stats = Model.GetComponentInChildren<StatsController>(true);
        Stats.Initialize(this);
        
        Panel.SetValues(lifestyle, true);
    }

    /// <summary>
    /// Switches to the next visualization.
    /// </summary>
    public void NextVisualization() {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                Activity.Toggle(false);
                Prius.Toggle(true);
                CurrentVisualization = Visualization.Prius;
                break;
            case Visualization.Prius:
                // Switch to Stats
                Prius.Toggle(false);
                Stats.Toggle(true);
                CurrentVisualization = Visualization.Stats;
                break;
            case Visualization.Stats:
                // Switch to Activity
                // TODO: fix bug in switching to activity
                Stats.Toggle(false);
                Activity.Toggle(true);
                CurrentVisualization = Visualization.Activity;
                break;
        }
    }

    /// <summary>
    /// Updates the current visualization. This is usually caused by a change in year.
    /// </summary>
    /// <returns></returns>
    public bool UpdateVisualization() {
        return false;
    }
}
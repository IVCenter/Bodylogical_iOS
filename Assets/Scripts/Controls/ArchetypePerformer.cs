using System.Collections;
using UnityEngine;

/// <summary>
/// A wrapper class for the models in the visualizations.
/// </summary>
public class ArchetypePerformer : ArchetypeModel {
    public HealthChoice Choice;
    [SerializeField] private BackwardsProps props;
    
    public Lifestyle ArchetypeLifestyle { get; }
    public LongTermHealth ArchetypeHealth { get; }
    public SwitchIcon Icon { get; private set; }
    public ActivityController Activity { get; private set; }
    public PriusController Prius { get; private set; }
    public StatsController Stats { get; private set; }
    public Visualization CurrentVisualization { get; private set; } = Visualization.Activity;

    private void Start() {
        // TODO: initialize lifestyle and health
    
        Icon = model.GetComponentInChildren<SwitchIcon>();
        Icon.Initialize(this);
        Activity = model.GetComponentInChildren<ActivityController>(true);
        Activity.Initialize(this, props);
        Prius = model.GetComponentInChildren<PriusController>(true);
        Prius.Initialize(this);
        Stats = model.GetComponentInChildren<StatsController>(true);
        Stats.Initialize(this);
        
        panel.SetValues(ArchetypeLifestyle, true);
    }

    /// <summary>
    /// Switches to the next visualization.
    /// </summary>
    public IEnumerator NextVisualization() {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                yield return Activity.Toggle(false);
                yield return Prius.Toggle(true);
                CurrentVisualization = Visualization.Prius;
                break;
            case Visualization.Prius:
                // Switch to Stats
                yield return Prius.Toggle(false);
                yield return Stats.Toggle(true);
                CurrentVisualization = Visualization.Stats;
                break;
            case Visualization.Stats:
                // Switch to Activity
                yield return Stats.Toggle(false);
                yield return Activity.Toggle(true);
                CurrentVisualization = Visualization.Activity;
                break;
        }
        
        Icon.UpdateIcon();
    }

    /// <summary>
    /// Updates the current visualization. This is usually caused by a change in year.
    /// </summary>
    public void UpdateVisualization() {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                Activity.Visualize(TimeProgressManager.Instance.YearValue / 5);
                break;
            case Visualization.Prius:
                // Switch to Stats
                Prius.Visualize(TimeProgressManager.Instance.YearValue / 5);
                break;
            // There is no update for Stats
        }
    }

    public void Dispose() {
        // There is no need to reset the status of visualization controllers because we will destroy the performer object
        Object.Destroy(model);
    }
}
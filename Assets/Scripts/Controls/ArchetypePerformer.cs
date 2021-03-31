using System.Collections;
using UnityEngine;

/// <summary>
/// A wrapper class for the models in the visualizations.
/// </summary>
public class ArchetypePerformer : ArchetypeModel {
    public HealthChoice choice;
    [SerializeField] private BackwardsProps props;

    public Lifestyle ArchetypeLifestyle { get; set; }
    public LongTermHealth ArchetypeHealth { get; } = new LongTermHealth();
    public SwitchIcon icon;
    public ActivityController activity;
    public PriusController prius;
    public StatsController stats;
    public Visualization CurrentVisualization { get; private set; } = Visualization.Activity;

    private bool initialized;
    public void Initialize() {
        if (initialized) {
            return;
        }

        initialized = true;
        
        activity.Initialize(this, props);
        prius.Initialize(this);
        stats.Initialize(this);
        // TODO
        //panel.SetValues(ArchetypeHealth, true);

        if (choice != HealthChoice.Custom) {
            ArchetypeLifestyle = HealthUtil.Lifestyles[choice];
        } else {
            ArchetypeLifestyle = new Lifestyle();
        }
    }

    /// <summary>
    /// Switches to the next visualization.
    /// </summary>
    public IEnumerator NextVisualization() {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                yield return activity.Toggle(false);
                yield return prius.Toggle(true);
                CurrentVisualization = Visualization.Prius;
                break;
            case Visualization.Prius:
                // Switch to Stats
                yield return prius.Toggle(false);
                yield return stats.Toggle(true);
                CurrentVisualization = Visualization.Stats;
                break;
            case Visualization.Stats:
                // Switch to Activity
                yield return stats.Toggle(false);
                yield return activity.Toggle(true);
                CurrentVisualization = Visualization.Activity;
                break;
        }

        icon.UpdateIcon();
    }

    /// <summary>
    /// Updates the current visualization. This is usually caused by a change in year.
    /// </summary>
    public void UpdateVisualization() {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                activity.Visualize(TimeProgressManager.Instance.YearValue / 5);
                break;
            case Visualization.Prius:
                // Switch to Stats
                prius.Visualize(TimeProgressManager.Instance.YearValue / 5);
                break;
            // There is no update for Stats
        }
    }
}
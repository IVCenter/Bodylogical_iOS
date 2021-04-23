using System.Collections;
using UnityEngine;

/// <summary>
/// A wrapper class for the models in the visualizations.
/// </summary>
public class ArchetypePerformer : ArchetypeModel {
    public HealthChoice choice;
    [SerializeField] private BackwardsProps props;
    public SwitchIcon icon;
    public ActivityController activity;
    public PriusController prius;
    public StatsController stats;

    public Lifestyle ArchetypeLifestyle { get; private set; }
    public LongTermHealth ArchetypeHealth { get; private set; }
    public Visualization CurrentVisualization { get; set; } = Visualization.None;
    /// <summary>
    /// Used to indicate whether the health data is successfully downloaded from the server.
    /// </summary>
    public bool DataReady { get; set; }
    
    private bool initialized;

    public void Initialize() {
        if (initialized) {
            return;
        }

        initialized = true;

        activity.Initialize(this, props);
        prius.Initialize(this);
        stats.Initialize(this);

        ArchetypeHealth = new LongTermHealth {choice = choice};
        ArchetypeLifestyle = choice != HealthChoice.Custom ? HealthUtil.Lifestyles[choice] : new Lifestyle();
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
    public void UpdateVisualization(float index) {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                activity.Visualize(index);
                break;
            case Visualization.Prius:
                // Switch to Stats
                prius.Visualize(index);
                break;
            case Visualization.Stats:
                panel.UpdateStats(index);
                break;
        }
    }

    /// <summary>
    /// Connects to the server and updates the avatar's health based on the lifestyle.
    /// </summary>
    /// <param name="error">Stores potential network errors.</param>
    /// <param name="lifestyle">If no lifestyle is provided, it will use the performer's own avatar;
    /// otherwise, it will use the input.</param>
    /// <param name="silent">If true, then do not display any error messages in case of an error.</param>
    public IEnumerator QueryHealth(NetworkError error, Lifestyle lifestyle = null, bool silent = false) {
        if (lifestyle != null) {
            ArchetypeLifestyle = lifestyle;
        }

        DataReady = false;
        
        // Connect to the API and retrieve the data
        while (error.status != NetworkStatus.Success) {
            yield return NetworkUtils.Forecast(ArchetypeData, ArchetypeLifestyle, ArchetypeHealth, error);

            if (error.status == NetworkStatus.ServerError) {
                if (!silent) {
                    TutorialManager.Instance.ShowInstruction(error.MsgKey);
                }
            } else {
                break;
            }
        }

        DataReady = true;
    }

    public void UpdateStats() {
        panel.SetValues(ArchetypeHealth);
        stats.BuildStats();
        UpdateVisualization(TimeProgressManager.Instance.Index);
    }
}
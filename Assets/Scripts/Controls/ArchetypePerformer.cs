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
    
    public Lifestyle ArchetypeLifestyle { get; set; }
    public LongTermHealth ArchetypeHealth { get; private set; }
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
    public void UpdateVisualization() {
        switch (CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                activity.Visualize(TimeProgressManager.Instance.Index);
                break;
            case Visualization.Prius:
                // Switch to Stats
                prius.Visualize(TimeProgressManager.Instance.Index);
                break;
            // There is no update for Stats
        }
    }

    public IEnumerator UpdateHealth(Lifestyle lifestyle) {
        ArchetypeLifestyle = lifestyle;

        // Lock the buttons and show a loading text
        ControlPanelManager.Instance.LPanel.LockButtons(true);
        TutorialManager.Instance.ShowInstruction("Instructions.CalculateData");

        // Connect to the API and retrieve the data
        NetworkError error = new NetworkError();

        yield return NetworkUtils.Forecast(ArchetypeData, ArchetypeLifestyle, ArchetypeHealth, error);

        // Unlock the buttons and hide loading text
        ControlPanelManager.Instance.LPanel.LockButtons(false);
        TutorialManager.Instance.ClearInstruction();

        if (!error.success) {
            StartCoroutine(ShowErrorInstruction(error.message));
            yield break;
        }

        // Show the data on the panel
        panel.SetValues(ArchetypeHealth);
        stats.BuildStats();
        UpdateVisualization();
        yield return null;
    }

    private IEnumerator ShowErrorInstruction(string message) {
        TutorialManager.Instance.ShowInstruction("Instructions.NetworkError", new LocalizedParam(message));
        yield return new WaitForSeconds(5);
        TutorialManager.Instance.ClearInstruction();
    }
}
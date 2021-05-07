using UnityEngine;

/// <summary>
/// A manager that controls the archetypes.
/// </summary>
public class ArchetypeManager : MonoBehaviour {
    public static ArchetypeManager Instance { get; private set; }

    /// <summary>
    /// Position for tutorial.
    /// </summary>
    [SerializeField] private Transform tutorialTransform;

    public ArchetypeDisplayer displayer;
    public ArchetypePerformer[] performers;

    private ArchetypePerformer customPerformer;

    public ArchetypePerformer Performer {
        get {
            if (customPerformer == null) {
                foreach (ArchetypePerformer performer in performers) {
                    if (performer.choice == HealthChoice.Custom) {
                        customPerformer = performer;
                    }
                }
            }

            return customPerformer;
        }
    }

    public bool DataReady {
        get {
            foreach (ArchetypePerformer performer in performers) {
                if (!performer.DataReady) {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Syncs the subject id from the displayer to the performers.
    /// </summary>
    public void SyncArchetype() {
        foreach (ArchetypePerformer performer in performers) {
            performer.ArchetypeData = displayer.ArchetypeData;
        }
    }

    #region Tutorials

    public void HealthDataTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.HealthDataTitle", "Tutorials.HealthDataText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform,
            () => displayer.panel.AllClicked,
            postCallback: VisualizationTutorial);
    }

    private void VisualizationTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.VisualizationTitle", "Tutorials.VisualizationText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform,
            () => AppStateManager.Instance.CurrState == AppState.Visualizations,
            postCallback: StageManager.Instance.ActivityTutorial
        );
    }

    #endregion
}
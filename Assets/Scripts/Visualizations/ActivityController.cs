using UnityEngine;

public class ActivityController : MonoBehaviour {
    public JoggingVisualizer visualizer;
    public HeartIndicator heart;

    // Tutorials
    [SerializeField] private Transform activityTutorialTransform;
    public bool TutorialShown { get; set; }

    private ArchetypePerformer performer;

    public void Initialize(ArchetypePerformer archetypePerformer, BackwardsProps props) {
        heart.Initialize();
        performer = archetypePerformer;
        visualizer.Performer = archetypePerformer;
        visualizer.Props = props;
    }
    
    public void Toggle(bool on) {
        if (on) {
            heart.gameObject.SetActive(true);
        } else {
            heart.gameObject.SetActive(false);
            visualizer.Stop();
        }
    }

    /// <summary>
    /// Switch to Activity view.
    /// </summary>
    public void StartActivity() {
        // if (!TutorialShown) {
        //     TutorialParam text = new TutorialParam("Tutorials.ActivityTitle", "Tutorials.ActivityText");
        //     TutorialManager.Instance.ShowTutorial(text, activityTutorialTransform,
        //         () => TimeProgressManager.Instance.Playing, postCallback: TimeProgressManager.Instance.ShowTut1);
        //     TutorialShown = true;
        // }

        visualizer.Visualize(TimeProgressManager.Instance.YearValue / 5, performer.Choice);
    }
    
    public void ResetController() {
        visualizer.ResetVisualizer();
    }
}
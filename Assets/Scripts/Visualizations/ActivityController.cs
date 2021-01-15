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
        visualizer.Initialize(archetypePerformer);
        visualizer.Props = props;
    }
    
    public void Toggle(bool on) {
        if (on) {
            gameObject.SetActive(true);
            visualizer.Visualize(TimeProgressManager.Instance.YearValue / 5, performer.Choice);
            
            // if (!TutorialShown) {
            //     TutorialParam text = new TutorialParam("Tutorials.ActivityTitle", "Tutorials.ActivityText");
            //     TutorialManager.Instance.ShowTutorial(text, activityTutorialTransform,
            //         () => TimeProgressManager.Instance.Playing, postCallback: TimeProgressManager.Instance.ShowTut1);
            //     TutorialShown = true;
            // }
        } else {
            gameObject.SetActive(false);
            visualizer.Stop();
        }
    }

    public void ResetController() {
        visualizer.ResetVisualizer();
    }
}
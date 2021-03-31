using System.Collections;
using UnityEngine;

public class ActivityController : MonoBehaviour {
    public JoggingVisualizer visualizer;
    public HeartIndicator heart;
    
    private ArchetypePerformer performer;
    
    // Tutorials
    [SerializeField] private Transform activityTutorialTransform;
    public bool TutorialShown { get; set; }

    public void Initialize(ArchetypePerformer archetypePerformer, BackwardsProps props) {
        performer = archetypePerformer;
        heart.Initialize();
        visualizer.Initialize(performer);
        visualizer.Props = props;
    }
    
    public IEnumerator Toggle(bool on) {
        if (on) {
            gameObject.SetActive(true);
            Visualize(TimeProgressManager.Instance.YearValue / 5);
            
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

        yield return null;
    }

    public bool Visualize(float year) {
        return visualizer.Visualize(year, performer.choice);
    }

    public void ResetController() {
        visualizer.ResetVisualizer();
    }
}
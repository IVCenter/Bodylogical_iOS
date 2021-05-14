using System.Collections;
using UnityEngine;

public class ActivityController : MonoBehaviour {
    public JoggingVisualizer visualizer;
    public HeartIndicator heart;
    
    private ArchetypePerformer performer;

    public void Initialize(ArchetypePerformer archetypePerformer, BackwardsProps props) {
        performer = archetypePerformer;
        heart.Initialize();
        visualizer.Initialize(performer);
        visualizer.Props = props;
    }
    
    public IEnumerator Toggle(bool on) {
        if (on) {
            gameObject.SetActive(true);
            Visualize(TimeProgressManager.Instance.Index);
        } else {
            gameObject.SetActive(false);
            visualizer.Stop();
        }

        yield return null;
    }

    public bool Visualize(float year) {
        return visualizer.Visualize(year);
    }

    public void ResetController() {
        visualizer.ResetVisualizer();
    }
}
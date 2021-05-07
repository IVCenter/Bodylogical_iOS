using System.Collections;
using UnityEngine;

public class PriusController : MonoBehaviour {
    public ColorLibrary colorLibrary;
    [SerializeField] private OrganVisualizer[] visualizers;

    private ArchetypePerformer performer;

    public void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
        foreach (OrganVisualizer visualizer in visualizers) {
            visualizer.Initialize(performer);
        }
    }

    public IEnumerator Toggle(bool on) {
        if (on) {
            gameObject.SetActive(true);
            Visualize(TimeProgressManager.Instance.Index);
        } else {
            gameObject.SetActive(false);
        }

        yield return null;
    }

    /// <summary>
    /// Play the prius visualization.
    /// </summary>
    /// <returns>true if the something so important happens that the time progression needs to be paused for closer
    /// inspection.</returns>
    public bool Visualize(float index) {
        bool changed = false;
        foreach (OrganVisualizer visualizer in visualizers) {
            changed = visualizer.Visualize(index) || changed;
        }

        return changed;
    }
}
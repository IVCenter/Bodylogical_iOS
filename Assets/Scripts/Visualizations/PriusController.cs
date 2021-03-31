using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PriusController : MonoBehaviour {
    public ColorLibrary colorLibrary;
    [SerializeField] private OrganVisualizer[] visualizers;
    [SerializeField] private Transform priusTutorialTransform;
    
    private ArchetypePerformer performer;
    
    public bool TutorialShown { get; set; }

    public void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
        foreach (OrganVisualizer visualizer in visualizers) {
            visualizer.Initialize(performer);
        }
    }
    
    public IEnumerator Toggle(bool on) {
        if (on) {
            gameObject.SetActive(true);
            Visualize(TimeProgressManager.Instance.YearValue / 5);

            // if (!TutorialShown) {
            //     TutorialParam text = new TutorialParam("Tutorials.PriusTitle", "Tutorials.PriusText");
            //     TutorialManager.Instance.ShowTutorial(text, priusTutorialTransform,
            //         () => displayInternals.AvatarHidden, postCallback: displayInternals.ShowTut1);
            //     TutorialShown = true;
            // }
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
            changed = visualizer.Visualize(index, performer.choice) || changed;
        }

        return changed;
    }
}
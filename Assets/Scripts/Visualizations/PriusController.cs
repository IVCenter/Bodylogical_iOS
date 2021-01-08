using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PriusController : MonoBehaviour {
    public ColorLibrary colorLibrary;
    [SerializeField] private OrganVisualizer[] visualizers;
    [SerializeField] private GameObject canvas;
    [SerializeField] private DisplayInternals displayInternals;
    [SerializeField] private Transform priusTutorialTransform;
    
    public bool TutorialShown { get; set; }

    private ArchetypePerformer performer;
    
    private void Start() {
        displayInternals.Initialize(); // Only executes once
    }

    public void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
    }
    
    public void Toggle(bool on) {
        if (on) {
            gameObject.SetActive(true);
            Visualize(TimeProgressManager.Instance.YearValue / 5);
            //displayInternals.Reset();
            SetExplanationText();

            // if (!TutorialShown) {
            //     TutorialParam text = new TutorialParam("Tutorials.PriusTitle", "Tutorials.PriusText");
            //     TutorialManager.Instance.ShowTutorial(text, priusTutorialTransform,
            //         () => displayInternals.AvatarHidden, postCallback: displayInternals.ShowTut1);
            //     TutorialShown = true;
            // }
        } else {
           gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Play the prius visualization.
    /// </summary>
    /// <returns>true if the something so important happens that the time progression needs to be paused for closer
    /// inspection.</returns>
    public bool Visualize(float index) {
        displayInternals.SetParticleColor(index);
        bool changed = false;
        foreach (OrganVisualizer visualizer in visualizers) {
            changed = visualizer.Visualize(index, performer.Choice) || changed;
        }

        return changed;
    }

    /// <summary>
    /// Sets the explanation text.
    /// </summary>
    public void SetExplanationText() {
        StringBuilder builder = new StringBuilder();
        foreach (OrganVisualizer visualizer in visualizers) {
            builder.AppendLine(visualizer.ExplanationText);
        }
        canvas.transform.Search("Explanation Text").GetComponent<Text>().text = builder.ToString();
    }
}
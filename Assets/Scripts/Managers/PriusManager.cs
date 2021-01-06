using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager to control the Prius visualization, a.k.a. internals.
/// </summary>
public class PriusManager : MonoBehaviour {
    public static PriusManager Instance { get; private set; }

    public GameObject priusParent;

    public ColorLibrary colorLibrary;
    [SerializeField] private OrganVisualizer[] visualizers;
    [SerializeField] private GameObject canvas;
    [SerializeField] private DisplayInternals displayInternals;
    [SerializeField] private Transform priusTutorialTransform;
    public bool TutorialShown { get; set; }

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        displayInternals.Initialize();
    }

    public void StartPrius() {
        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
        displayInternals.Reset();
        SetExplanationText();

        if (!TutorialShown) {
            TutorialParam text = new TutorialParam("Tutorials.PriusTitle", "Tutorials.PriusText");
            TutorialManager.Instance.ShowTutorial(text, priusTutorialTransform,
                () => displayInternals.AvatarHidden, postCallback: displayInternals.ShowTut1);
            TutorialShown = true;
        }
    }

    /// <summary>
    /// Play the prius visualization.
    /// </summary>
    /// <returns>true if the something so important happens that the time progression needs to be paused for closer
    /// inspection.</returns>
    public bool Visualize(float index, HealthChoice choice) {
        displayInternals.SetParticleColor(index);
        bool changed = false;
        foreach (OrganVisualizer visualizer in visualizers) {
            changed = visualizer.Visualize(index, choice) || changed;
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

    public void ResetManager() {
        priusParent.SetActive(false);
    }
}
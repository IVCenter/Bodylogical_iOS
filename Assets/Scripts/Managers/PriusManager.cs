using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager to control the Prius visualization, a.k.a. internals.
/// </summary>
public class PriusManager : MonoBehaviour {
    public static PriusManager Instance { get; private set; }

    public GameObject priusParent;

    [SerializeField] private PriusVisualizer priusVisualizer;
    [SerializeField] private GameObject canvas;
    [SerializeField] private DisplayInternals displayInternals;

    public GameObject LegendPanel => canvas.transform.Search("Legend Panel").gameObject;
    public Text ExplanationText => canvas.transform.Search("Explanation Text").GetComponent<Text>();

    [SerializeField] private Transform priusTutorialTransform;
    [HideInInspector] public bool tutorialShown;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        displayInternals.Initialize();
    }

    public IEnumerator StartPrius(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, priusParent, true);

        displayInternals.Reset();
        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
        SetExplanationText();

        if (!tutorialShown) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam text = new TutorialParam("Tutorials.PriIntroTitle", "Tutorials.PriIntroText");
            TutorialManager.Instance.ShowTutorial(text, priusTutorialTransform);
            tutorialShown = true;
        }
    }

    /// <summary>
    /// Play the prius visualization.
    /// </summary>
    /// <returns><c>true</c> if the something so important happens that the time progression needs to be paused for closer inspection.</returns>
    public bool Visualize(float index, HealthChoice choice) {
        return priusVisualizer.Visualize(index, choice);
    }

    /// <summary>
    /// Sets the explanation text.
    /// </summary>
    public void SetExplanationText() {
        ExplanationText.text = priusVisualizer.ExplanationText;
    }

    public void Reset() {
        priusParent.SetActive(false);
    }
}
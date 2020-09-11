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

    public IEnumerator StartPrius(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, priusParent, true);
        
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
        return priusVisualizer.Visualize(index, choice);
    }

    /// <summary>
    /// Sets the explanation text.
    /// </summary>
    public void SetExplanationText() {
        canvas.transform.Search("Explanation Text").GetComponent<Text>().text = priusVisualizer.ExplanationText;
    }

    public void Reset() {
        priusParent.SetActive(false);
    }
}
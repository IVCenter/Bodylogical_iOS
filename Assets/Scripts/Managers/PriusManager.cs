using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager to control the Prius visualization, a.k.a. internals.
/// </summary>
public class PriusManager : MonoBehaviour {
    public static PriusManager Instance { get; private set; }

    public GameObject femaleXRay, maleXRay;
    public GameObject priusParent;
    public PriusVisualizer priusVisualizer;
    public GameObject canvas;

    public GameObject LegendPanel { get { return canvas.transform.Search("Legend Panel").gameObject; } }
    public Text ExplanationText { get { return canvas.transform.Search("Explanation Text").GetComponent<Text>(); } }
    [HideInInspector]
    public PriusType currentPart;
    [HideInInspector]
    public bool kidneyLeft;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// Notice: does NOT toggle parent object (left to StartPrius).
    /// </summary>
    public void TogglePrius(bool on) {
        ControlPanelManager.Instance.TogglePriusSelector(on);
        ControlPanelManager.Instance.ToggleTimeControls(on);

        // if toggle off, hide both models; else show the one with the corresponding gender.
        bool isFemale = HumanManager.Instance.SelectedArchetype.gender == Gender.Female;
        maleXRay.SetActive(!isFemale);
        femaleXRay.SetActive(isFemale);
    }

    public IEnumerator StartPrius(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, priusParent, true);

        currentPart = PriusType.Human;
        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
        SetExplanationText();
        yield return null;
    }

    /// <summary>
    /// When the heart part is clicked, move the heart to the top, and show the circulation.
    /// </summary>
    public void ToggleHeart() {
        bool isHeart = currentPart == PriusType.Heart;
        currentPart = isHeart ? PriusType.Human : PriusType.Heart;
        LegendPanel.SetActive(isHeart);
        priusVisualizer.MoveOrgan(!isHeart, PriusType.Heart);
        SetExplanationText();
    }

    /// <summary>
    /// When the kidney part is clicked, move the kidney to the top, and show the kidney.
    /// </summary>
    public void ToggleKidney(bool left) {
        bool prevLeft = kidneyLeft;
        kidneyLeft = left;
        bool isKidney = currentPart == PriusType.Kidney;
        bool differed = prevLeft != left;

        // if previous is kidney but is a DIFFERENT kidney, still keep current part
        // and play small-to-large animation
        bool stl = !isKidney || differed;
        currentPart = !stl ? PriusType.Human : PriusType.Kidney;
        LegendPanel.SetActive(isKidney);

        priusVisualizer.MoveOrgan(stl, PriusType.Kidney);

        SetExplanationText();
    }

    /// <summary>
    /// WHen the liver part is clicked, move the liver to the top, and show the liver.
    /// </summary>
    public void ToggleLiver() {
        bool isLiver = currentPart == PriusType.Liver;
        currentPart = isLiver ? PriusType.Human : PriusType.Liver;
        LegendPanel.SetActive(isLiver);
        priusVisualizer.MoveOrgan(!isLiver, PriusType.Liver);
        SetExplanationText();
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
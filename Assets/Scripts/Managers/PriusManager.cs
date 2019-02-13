using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager to control the Prius visualization, a.k.a. internals.
/// </summary>
public class PriusManager : MonoBehaviour {
    public static PriusManager Instance { get; private set; }

    public GameObject priusParent;
    public GameObject priusSelections;
    public GameObject heart;
    public GameObject kidney;
    public GameObject liver;
    public GameObject canvas;

    public GameObject LegendPanel { get { return canvas.transform.Search("Legend Panel").gameObject; } }
    public Text ExplanationText { get { return canvas.transform.Search("Explanation Text").GetComponent<Text>(); } }
    public PriusVisualizer Visualizer { get { return priusSelections.GetComponent<PriusVisualizer>(); } }

    private PriusType currentPart;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public IEnumerator StartPrius() {
        yield return HumanManager.Instance.MoveSelectedHumanToCenter();
        priusSelections.SetActive(true);
        currentPart = PriusType.Human;
        if (StageManager.Instance.Path == HealthChoice.NotSet) {
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use buttons to move through time", 3);
        }
        yield return null;
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void TogglePrius(bool on) {
        ButtonSequenceManager.Instance.SetInternals(!on);

        priusParent.SetActive(on);
        heart.SetActive(false);
        liver.SetActive(false);
        kidney.SetActive(false);
        canvas.SetActive(on);
        ButtonSequenceManager.Instance.SetTimeSlider(on);
        ButtonSequenceManager.Instance.SetPathButtons(on);

        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetPropsButton(on);
        ButtonSequenceManager.Instance.SetTimeSlider(on);
    }

    /// <summary>
    /// When the heart part is clicked, show the heart.
    /// </summary>
    public void ToggleHeart() {
        bool isHeart = currentPart == PriusType.Heart;
        currentPart = isHeart ? PriusType.Human : PriusType.Heart;
        HumanManager.Instance.SelectedHuman.SetActive(isHeart);
        priusSelections.SetActive(isHeart);
        heart.SetActive(!isHeart);
        LegendPanel.SetActive(isHeart);
        Visualizer.ShowOrgan(currentPart);
    }

    /// <summary>
    /// When the kidney part is clicked, show the kidney.
    /// </summary>
    public void ToggleKidney() {
        bool isKidney = currentPart == PriusType.Kidney;
        currentPart = isKidney ? PriusType.Human : PriusType.Kidney;
        HumanManager.Instance.SelectedHuman.SetActive(isKidney);
        priusSelections.SetActive(isKidney);
        kidney.SetActive(!isKidney);
        LegendPanel.SetActive(isKidney);
        Visualizer.ShowOrgan(currentPart);
    }

    /// <summary>
    /// WHen the liver part is clicked, show the liver.
    /// </summary>
    public void ToggleLiver() {
        bool isLiver = currentPart == PriusType.Liver;
        currentPart = isLiver ? PriusType.Human : PriusType.Liver;
        HumanManager.Instance.SelectedHuman.SetActive(isLiver);
        priusSelections.SetActive(isLiver);
        liver.SetActive(!isLiver);
        LegendPanel.SetActive(isLiver);
        Visualizer.ShowOrgan(currentPart);
    }

    /// <summary>
    /// Play the prius visualization.
    /// </summary>
    /// <returns><c>true</c> if the something so important happens that the time progression needs to be paused for closer inspection.</returns>
    public bool Visualize(int index, HealthChoice choice) {
        return Visualizer.Visualize(index, choice);
    }

    /// <summary>
    /// Sets the explanation text.
    /// </summary>
    public void SetExplanationText() {
        ExplanationText.text = Visualizer.ExplanationText;
    }
}
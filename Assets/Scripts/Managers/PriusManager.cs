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
    public Transform xRayParent;

    [SerializeField] private GameObject femaleXRay, maleXRay;
    [HideInInspector] public GameObject currentXRay;
    [SerializeField] private PriusVisualizer priusVisualizer;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Switcher priusSwitcher;

    private Gender xRayGender = Gender.Either;

    public GameObject LegendPanel => canvas.transform.Search("Legend Panel").gameObject;
    public Text ExplanationText => canvas.transform.Search("Explanation Text").GetComponent<Text>();

    [HideInInspector] public PriusType currentPart;

    [SerializeField] private Transform priusTutorialTransform;
    [HideInInspector] public bool tutorialShown;

    private Dictionary<PriusType, System.Action> toggles;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        // If it switches to human, just call currentPart's toggle to return to human.
        toggles = new Dictionary<PriusType, System.Action> {
            { PriusType.Heart, ToggleHeart },
            { PriusType.Kidney, ToggleKidney },
            { PriusType.Liver, ToggleLiver }
        };
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// Notice: does NOT toggle parent object (left to StartPrius).
    /// </summary>
    public void TogglePrius(bool on) {
        //ControlPanelManager.Instance.TogglePriusSelector(on);
        //ControlPanelManager.Instance.ToggleTimeControls(on);

        // if toggle off, hide both models; else show the one with the corresponding gender.
        bool isFemale = ArchetypeManager.Instance.selectedArchetype.gender == Gender.Female;
        if (ArchetypeManager.Instance.selectedArchetype.gender != xRayGender) {
            if (currentXRay != null) {
                Destroy(currentXRay);
            }
            currentXRay = Instantiate(isFemale ? femaleXRay : maleXRay, xRayParent);
        }
    }

    public IEnumerator StartPrius(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, priusParent, true);

        currentPart = PriusType.Human;
        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
        SetExplanationText();

        if (!tutorialShown) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam text = new TutorialParam("Tutorials.PriIntroTitle", "Tutorials.PriIntroText");
            TutorialManager.Instance.ShowTutorial(text, priusTutorialTransform);
            tutorialShown = true;
        }
    }

    public void ToggleOrgan(int index) {
        PriusType type = (PriusType)index;
        if (type == PriusType.Human) {
            toggles[currentPart]();
        } else {
            toggles[(PriusType)index]();
        }
    }

    /// <summary>
    /// When the heart part is clicked, move the heart to the top, and show the circulation.
    /// </summary>
    public void ToggleHeart() {
        bool isHeart = currentPart == PriusType.Heart;
        currentPart = isHeart ? PriusType.Human : PriusType.Heart;
        LegendPanel.SetActive(isHeart);
        StageManager.Instance.header.SetActive(isHeart);
        priusVisualizer.MoveOrgan(!isHeart, PriusType.Heart);
        SetExplanationText();
        priusSwitcher.Switch((int)currentPart);
    }

    /// <summary>
    /// When the kidney part is clicked, move the kidney to the top, and show the kidney.
    /// </summary>
    public void ToggleKidney() {
        bool isKidney = currentPart == PriusType.Kidney;
        currentPart = isKidney ? PriusType.Human : PriusType.Kidney;
        LegendPanel.SetActive(isKidney);
        StageManager.Instance.header.SetActive(isKidney);
        priusVisualizer.MoveOrgan(!isKidney, PriusType.Kidney);
        SetExplanationText();
        priusSwitcher.Switch((int)currentPart);
    }

    /// <summary>
    /// WHen the liver part is clicked, move the liver to the top, and show the liver.
    /// </summary>
    public void ToggleLiver() {
        bool isLiver = currentPart == PriusType.Liver;
        currentPart = isLiver ? PriusType.Human : PriusType.Liver;
        LegendPanel.SetActive(isLiver);
        StageManager.Instance.header.SetActive(isLiver);
        priusVisualizer.MoveOrgan(!isLiver, PriusType.Liver);
        SetExplanationText();
        priusSwitcher.Switch((int)currentPart);
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
        Destroy(currentXRay);
        Debug.Log("Destroyed");
    }
}
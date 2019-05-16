using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;
    [Header("Activity index MUST match control panel dropdown index.")]
    public List<GameObject> activities;
    public LocalizedText buttonText;
    public CompanionController maleController;
    public CompanionController femaleController;
    
    public CompanionController CurrentCompanion { 
        get { 
            return HumanManager.Instance.SelectedArchetype.gender == Gender.Male ?
                maleController : femaleController;
        }
    }

    public CompanionController OtherCompanion {
        get {
            return HumanManager.Instance.SelectedArchetype.gender == Gender.Male ?
                femaleController : maleController;
        }
    }

    public Transform CurrentTransform { get { return CurrentCompanion.transform; } }
    public Transform OtherTransform { get { return OtherCompanion.transform; } }
    public Animator CurrentAnimator { get { return CurrentCompanion.CurrentAnimator; } }
    public Animator OtherAnimator { get { return OtherCompanion.CurrentAnimator; } }

    public WheelchairController wheelchair;

    public DropDownInteract activityDropdown;

    public HeartIndicator charHeart, compHeart;

    private List<Visualizer> visualizers;
    private int currentIndex;
    private bool initialized;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        visualizers = new List<Visualizer>();
        foreach (GameObject activity in activities) {
            visualizers.Add(activity.GetComponent<Visualizer>());
        }
    }

    /// <summary>
    /// Switch to Animations view.
    /// </summary>
    public IEnumerator StartActivity(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, activityParent);
        // after stage is shown
        yield return HumanManager.Instance.MoveSelectedHumanToLeft();
        OtherCompanion.gameObject.SetActive(false);
        CurrentCompanion.gameObject.SetActive(true);
        Visualize(TimeProgressManager.Instance.YearCount / 5, TimeProgressManager.Instance.Path);
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// Notice: does NOT toggle parent object (left to StartActivity).
    /// </summary>
    public void ToggleActivity(bool on) {
        ButtonSequenceManager.Instance.SetActivitiesButton(!on);

        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetPriusButton(on);
        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetActivityFunction(on);
        visualizers[currentIndex].Pause();
        buttonText.SetText("Buttons.ActCurrent", new LocalizedParam(visualizers[currentIndex].VisualizerKey, true));
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(int index, HealthChoice choice) {
        if (!initialized) {
            visualizers[currentIndex].Initialize();
            initialized = true;
        }

        compHeart.Display(HealthStatus.Good);
        visualizers[currentIndex].Visualize(index, choice);
    }

    public void SwitchActivity(int index) {
        buttonText.SetText("Buttons.ActCurrent", new LocalizedParam(visualizers[index].VisualizerKey, true));
        visualizers[currentIndex].Pause();
        activities[currentIndex].SetActive(false);
        currentIndex = index;
        activities[currentIndex].SetActive(true);
        if (initialized) {
            visualizers[currentIndex].Initialize();
            compHeart.Display(HealthStatus.Good);
            visualizers[currentIndex].Visualize(TimeProgressManager.Instance.YearCount / 5, TimeProgressManager.Instance.Path);
        }
    }
      
    public void Reset() {
        initialized = false;
        activityDropdown.OnOptionClicked(0);
    }
}
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
    public Text buttonText;
    public CompanionController companionController;
    public Transform CompanionTransform { get { return companionController.transform; } }
    public Animator CompanionAnimator { get { return companionController.CurrentAnimator; } }
    public DropDownInteract activityDropdown;

    private List<Visualizer> visualizers;
    private int currentIndex;
    private bool isLeft;
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
    public IEnumerator StartAnimations() {
        yield return HumanManager.Instance.MoveSelectedHumanToLeft();
        isLeft = true;
        Visualize(TimeProgressManager.Instance.YearCount / 5, TimeProgressManager.Instance.Path);
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void ToggleAnimation(bool on) {
        ButtonSequenceManager.Instance.SetActivitiesButton(!on);

        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetPriusButton(on);
        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetActivityFunction(on);

        activityParent.SetActive(on);
        isLeft = false;
        visualizers[currentIndex].Pause();
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(int index, HealthChoice choice) {
        if (!isLeft) {
            HumanManager.Instance.MoveSelectedHumanToLeft();
            isLeft = true;
        }
        if (!initialized) {
            visualizers[currentIndex].Initialize();
            initialized = true;
        }
        visualizers[currentIndex].Visualize(index, choice);
    }

    public void SwitchActivity(int index) {
        buttonText.text = "Current: " + visualizers[index].VisualizerName;
        visualizers[currentIndex].Pause();
        activities[currentIndex].SetActive(false);
        currentIndex = index;
        activities[currentIndex].SetActive(true);
        if (initialized) {
            visualizers[currentIndex].Initialize();
            visualizers[currentIndex].Visualize(TimeProgressManager.Instance.YearCount / 5, TimeProgressManager.Instance.Path);
        }
    }

    public void Reset() {
        initialized = false;
        activityDropdown.OnOptionClicked(0);
    }
}
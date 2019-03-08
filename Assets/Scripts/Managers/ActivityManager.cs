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
    private List<GameObject> activities;
    private List<Visualizer> visualizers;
    public Text buttonText;

    public DropDownInteract activityDropdown;
    private int currentIndex;
    private readonly Dictionary<int, string> activityNameDictionary = new Dictionary<int, string> {
        {0, "Jogging"},
        {1, "Soccer"}
    };
    private bool isLeft;
    private bool initialized = false;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        activities = new List<GameObject>();
        visualizers = new List<Visualizer>();
        foreach (Transform activityTransform in activityParent.transform) {
            activities.Add(activityTransform.gameObject);
            visualizers.Add(activityTransform.GetComponent<Visualizer>());
        }
    }

    /// <summary>
    /// Switch to Animations view.
    /// </summary>
    public IEnumerator StartAnimations() {
        yield return HumanManager.Instance.MoveSelectedHumanToLeft();
        isLeft = true;
        Visualize(TimeProgressManager.Instance.Year / 5, TimeProgressManager.Instance.Path);
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void ToggleAnimation(bool on) {
        ButtonSequenceManager.Instance.SetActivitiesButton(!on);

        activityParent.SetActive(false); // only appears after selecting a path
        ButtonSequenceManager.Instance.SetTimeControls(on);

        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetPriusButton(on);
        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetActivityFunction(on);
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
        activityParent.SetActive(true);
        visualizers[currentIndex].Visualize(index, choice);
    }

    public void SwitchActivity(int index) {
        buttonText.text = "Current: " + activityNameDictionary[index];
        visualizers[currentIndex].Pause();
        activities[currentIndex].SetActive(false);
        currentIndex = index;
        activities[currentIndex].SetActive(true);
        if (initialized) {
            visualizers[currentIndex].Initialize();
            visualizers[currentIndex].Visualize(TimeProgressManager.Instance.Year / 5, TimeProgressManager.Instance.Path);
        }
    }

    public void Reset() {
        initialized = false;
        activityDropdown.OnOptionClicked(0);
    }
}
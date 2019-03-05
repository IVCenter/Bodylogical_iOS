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
    
    private int currentActivity;
    private readonly Dictionary<int, string> activityNameDictionary = new Dictionary<int, string> {
        {0, "Jogging"},
        {1, "Soccer"}
    };
    private bool isLeft;
    private bool initialized = false;
    private bool isPlaying;

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
        if (TimeProgressManager.Instance.Path == HealthChoice.NotSet) {
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use buttons to move through time", 3);
        } else {
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
            isLeft = true;
            Visualize(0, TimeProgressManager.Instance.Path);
        }
        yield return null;
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
        visualizers[currentActivity].Pause();
        isPlaying = false;
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(int index, HealthChoice choice) {
        isPlaying = true;
        if (!isLeft) {
            HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        isLeft = true;
        if (!initialized) {
            visualizers[0].Initialize();
            initialized = true;
        }
        activityParent.SetActive(true);
        visualizers[currentActivity].Visualize(index, choice);
    }

    public void SwitchActivity(int index) {
        buttonText.text = "Current: " + activityNameDictionary[index];
        visualizers[currentActivity].Pause();
        activities[currentActivity].SetActive(false);
        currentActivity = index;
        activities[currentActivity].SetActive(true);
        if (isPlaying) {
            visualizers[currentActivity].Visualize(TimeProgressManager.Instance.Year, TimeProgressManager.Instance.Path);
        }
    }
}
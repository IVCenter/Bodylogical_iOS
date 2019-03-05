using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;
    private List<Visualizer> visualizers;

    private bool isLeft;
    private bool initialized = false;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        visualizers = new List<Visualizer>();
        foreach (Transform activityTransform in activityParent.transform) {
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

    //public void PauseAnimations() {
    //    visualizer.Pause();
    //}

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
        isLeft = false;
        visualizers[0].Pause();
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(int index, HealthChoice choice) {
        if (!isLeft) {
            HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        isLeft = true;
        if (!initialized) {
            visualizers[0].Initialize();
            initialized = true;
        }
        activityParent.SetActive(true);
        visualizers[0].Visualize(index, choice);
    }
}

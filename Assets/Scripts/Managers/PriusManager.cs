using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the Prius visualization, a.k.a. internals.
/// </summary>
public class PriusManager : MonoBehaviour {
    public static PriusManager Instance { get; private set; }

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void ToggleInternals() {
        AnimationManager.Instance.ToggleAnimation(false);
        StageManager.Instance.Reset();
        HumanManager.Instance.SelectedHuman.SetActive(false);
        ButtonSequenceManager.Instance.SetToggleButtons(false);
        ButtonSequenceManager.Instance.SetFunctionButtons(false);
        // need to switch back to line chart and props easily
        ButtonSequenceManager.Instance.SetLineChartButton(true);
        ButtonSequenceManager.Instance.SetPropsButton(true);
        // need to hide visualizations button and show sliders/paths buttons
        ButtonSequenceManager.Instance.SetInternals(false);
        ButtonSequenceManager.Instance.SetTimeSlider(true);
        ButtonSequenceManager.Instance.SetPathButtons(true);
        if (StageManager.Instance.Path == null) {
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use slider to move through time", 3);
        } // if a path is chosen show the internals
        TutorialText.Instance.ShowDouble("You have entered prius visualization", "Placeholder, nothing here", 3);
    }
}

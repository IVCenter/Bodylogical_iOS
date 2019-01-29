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

    public void StartPrius() {
        if (StageManager.Instance.Path == null) {
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use slider to move through time", 3);
        } // if a path is chosen show the internals
        TutorialText.Instance.ShowDouble("You have entered prius visualization", "Placeholder, nothing here", 3);
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void TogglePrius(bool on) {
        ButtonSequenceManager.Instance.SetInternals(!on);


        ButtonSequenceManager.Instance.SetTimeSlider(on);
        ButtonSequenceManager.Instance.SetPathButtons(on);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeProgressManager : MonoBehaviour {
    public static TimeProgressManager Instance { get; private set; }

    [SerializeField] private LocalizedText headerText;
    [SerializeField] private SliderInteract sliderInteract;
    [SerializeField] private Text sliderText;

    public bool Playing { get; private set; }
    private IEnumerator timeProgressCoroutine;

    public float YearValue { get; private set; }
    private int year;

    public const int MaxYears = 40;

    // Tutorial-related variables
    [SerializeField] private Transform timeTutorialTransform;
    private bool updated;
    private bool stopped;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// When the slider is changed, update the year.
    /// </summary>
    /// <param name="value">Value.</param>
    public void UpdateYear(float value) {
        updated = true;

        YearValue = value;
        if (Mathf.RoundToInt(value) != year) {
            year = Mathf.RoundToInt(value);
            UpdateHeaderText();
        }

        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.Performers.Values) {
            performer.UpdateVisualization();
        }

        // if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
        //     ActivityManager.Instance.Visualize(YearValue / 5, Path);
        // } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
        //     bool healthChange = PriusManager.Instance.Visualize(YearValue / 5, Path);
        //     if (healthChange) {
        //         PriusManager.Instance.SetExplanationText();
        //         if (Playing) {
        //             TimePlayPause();
        //             TutorialManager.Instance.ShowStatus("Instructions.PriHealthChange");
        //         }
        //     }
        // }
    }

    /// <summary>
    /// When "play/pause" button is clicked, start/stop time progression.
    /// </summary>
    public void TimePlayPause() {
        if (!Playing) {
            // stopped/paused, start
            timeProgressCoroutine = TimeProgress();
            StartCoroutine(timeProgressCoroutine);
        } else {
            // started, pause
            StopCoroutine(timeProgressCoroutine);
        }

        Playing = !Playing;
    }

    /// <summary>
    /// Update header text.
    /// </summary>
    public void UpdateHeaderText() {
        sliderText.text = year.ToString();
        headerText.SetText("Legends.HeaderText",
            new LocalizedParam(System.DateTime.Today.Year + year),
            new LocalizedParam(ArchetypeManager.Instance.Selected.ArchetypeData.age + year));
    }

    /// <summary>
    /// When "stop" button is clicked, stop and reset time progression.
    /// </summary>
    public void TimeStop() {
        stopped = true;

        if (Playing) {
            TimePlayPause();
        }

        UpdateYear(0);
        sliderInteract.SetSlider(0);
        // if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
        //     PriusManager.Instance.SetExplanationText();
        // }
    }

    /// <summary>
    /// Helper method to progress through time. Currently updates on a year to year basis.
    /// </summary>
    private IEnumerator TimeProgress() {
        while (YearValue <= MaxYears) {
            // update on a yearly basis
            if (Mathf.RoundToInt(YearValue) != year) {
                UpdateYear(YearValue);
            }

            sliderInteract.SetSlider(YearValue / MaxYears);

            yield return null;
            YearValue += Time.deltaTime;
        }

        // after loop, stop.
        Playing = false;
        UpdateYear(MaxYears);
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void ResetTime() {
        stopped = true;

        if (Playing) {
            TimePlayPause();
        }

        sliderInteract.SetSlider(0);
        YearValue = 0;
        year = 0;
        UpdateHeaderText();
    }

    #region Tutorials

    public void ShowTut1() {
        TutorialParam param = new TutorialParam("Tutorials.TimeTitle", "Tutorials.TimeText");
        TutorialManager.Instance.ShowTutorial(param, timeTutorialTransform, () => !Playing, postCallback: ShowTut2);
    }

    private void ShowTut2() {
        TutorialParam param = new TutorialParam("Tutorials.TimeTitle", "Tutorials.TimeText2");
        TutorialManager.Instance.ShowTutorial(param, timeTutorialTransform, () => stopped || updated,
            () => {
                stopped = false;
                updated = false;
            }, ShowTut3);
    }

    private void ShowTut3() {
        TutorialParam param = new TutorialParam("Tutorials.TimeTitle", "Tutorials.TimeText3");
        TutorialManager.Instance.ShowTutorial(param, timeTutorialTransform,
            () => {
                foreach (ArchetypePerformer performer in ArchetypeManager.Instance.Performers.Values) {
                    if (performer.CurrentVisualization == Visualization.Prius) {
                        return true;
                    }
                }

                return false;
            },
            postCallback: StageManager.Instance.PriusTutorial
        );
    }

    #endregion
}
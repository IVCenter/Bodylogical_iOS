using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeProgressManager : MonoBehaviour {
    public static TimeProgressManager Instance { get; private set; }

    [SerializeField] private LocalizedText headerText;
    [SerializeField] private SliderInteract sliderInteract;
    [SerializeField] private Text sliderText;

    public bool Playing { get; private set; }
    private IEnumerator timeProgressCoroutine;

    public HealthChoice Path { get; private set; }
    public float YearValue { get; private set; }
    private int year;

    public static readonly int maxYears = 20;

    public readonly Dictionary<HealthChoice, string> choicePathDictionary = new Dictionary<HealthChoice, string> {
        {HealthChoice.None, "General.PathRedVerbose"},
        {HealthChoice.Minimal, "General.PathYellowVerbose"},
        {HealthChoice.Optimal, "General.PathGreenVerbose"}
    };

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

        if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
            ActivityManager.Instance.Visualize(YearValue / 5, Path);
        } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
            bool healthChange = PriusManager.Instance.Visualize(YearValue / 5, Path);
            if (healthChange) {
                PriusManager.Instance.SetExplanationText();
                if (Playing) {
                    TimePlayPause();
                    TutorialManager.Instance.ShowStatus("Instructions.PriHealthChange");
                }
            }
        }
    }

    /// <summary>
    /// Switch the path based on the switcher.
    /// </summary>
    /// <param name="path">HealthChoice representation.</param>
    public void UpdatePath(int path) {
        Path = (HealthChoice) path;
        UpdateHeaderText();
        TutorialManager.Instance.ShowStatus("Instructions.PathSwitch",
            new LocalizedParam(choicePathDictionary[Path], true));

        if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
            ActivityManager.Instance.Visualize(YearValue / 5, Path);
        } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
            PriusManager.Instance.Visualize(YearValue / 5, Path);
            PriusManager.Instance.SetExplanationText();
        } else if (AppStateManager.Instance.CurrState == AppState.VisLineChart) {
            LineChartManager.Instance.Reload();
            ChoicePanelManager.Instance.SetValues();
        }
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
            new LocalizedParam(ArchetypeManager.Instance.Selected.ArchetypeData.age + year),
            new LocalizedParam(choicePathDictionary[Path], true));
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
        if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
            PriusManager.Instance.SetExplanationText();
        }
    }

    /// <summary>
    /// Helper method to progress through time. Currently updates on a year to year basis.
    /// </summary>
    private IEnumerator TimeProgress() {
        while (YearValue <= maxYears) {
            // update on a yearly basis
            if (Mathf.RoundToInt(YearValue) != year) {
                UpdateYear(YearValue);
            }

            sliderInteract.SetSlider(YearValue / maxYears);

            yield return null;
            YearValue += Time.deltaTime;
        }

        // after loop, stop.
        Playing = false;
        UpdateYear(maxYears);
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void Reset() {
        Path = HealthChoice.None;
        TimeStop();
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
            () => AppStateManager.Instance.CurrState == AppState.VisPrius);
    }

    #endregion
}
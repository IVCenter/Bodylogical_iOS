using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProgressManager : MonoBehaviour {
    public static TimeProgressManager Instance { get; private set; }

    public PlayPauseButton playPauseButton;
    public LocalizedText headerText;
    public SliderInteract sliderInteract;
    public LocalizedText sliderText;

    private bool isTimePlaying;
    private IEnumerator timeProgressCoroutine;

    public HealthChoice Path { get; private set; }

    public float YearValue { get; private set; }
    public int Year { get; private set; }

    //TODO: replace placeholder age with real age
    public int startAge = 20;

    public readonly Dictionary<HealthChoice, string> choicePathDictionary = new Dictionary<HealthChoice, string> {
        {HealthChoice.None, "General.PathRedVerbose"},
        {HealthChoice.Minimal, "General.PathYellowVerbose"},
        {HealthChoice.Optimal, "General.PathGreenVerbose"}
    };

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// When the slider is changed, update the year.
    /// </summary>
    /// <param name="value">Value.</param>
    public void UpdateYear(float value) {
        YearValue = value;
        if (Mathf.RoundToInt(value) != Year) {
            Year = Mathf.RoundToInt(value);
            UpdateHeaderText();
        }

        if (MasterManager.Instance.currPhase == GamePhase.VisActivity) {
            ActivityManager.Instance.Visualize(YearValue / 5, Path);
        } else if (MasterManager.Instance.currPhase == GamePhase.VisPrius) {
            bool healthChange = PriusManager.Instance.Visualize(YearValue / 5, Path);
            if (healthChange) {
                PriusManager.Instance.SetExplanationText();
                if (isTimePlaying) {
                    TimePlayPause();
                    TutorialManager.Instance.ShowStatus("Instructions.PriHealthChange");
                }
            }
        }
    }

    /// <summary>
    /// When the button is pressed, update the path.
    /// </summary>
    /// <param name="keyword">Keyword.</param>
    public void UpdatePath(string keyword) {
        Path = (HealthChoice)System.Enum.Parse(typeof(HealthChoice), keyword);
        UpdateHeaderText();
        TutorialManager.Instance.ShowStatus("Instructions.PathSwitch",
             new LocalizedParam(choicePathDictionary[Path], true));

        if (MasterManager.Instance.currPhase == GamePhase.VisActivity) {
            ActivityManager.Instance.Visualize(YearValue / 5, Path);
        } else if (MasterManager.Instance.currPhase == GamePhase.VisPrius) {
            PriusManager.Instance.Visualize(YearValue / 5, Path);
            PriusManager.Instance.SetExplanationText();
        }
    }

    /// <summary>
    /// When "play/pause" button is clicked, start/stop time progression.
    /// </summary>
    public void TimePlayPause() {
        if (!isTimePlaying) { // stopped/paused, start
            timeProgressCoroutine = TimeProgress();
            StartCoroutine(timeProgressCoroutine);
        } else { // started, pause
            StopCoroutine(timeProgressCoroutine);
        }
        isTimePlaying = !isTimePlaying;
        playPauseButton.ChangeImage(isTimePlaying);
    }

    /// <summary>
    /// Update header text.
    /// </summary>
    public void UpdateHeaderText() {
        sliderText.SetText("Legends.SliderText", new LocalizedParam(Year.ToString()));
        headerText.SetText("Legends.HeaderText",
            new LocalizedParam((System.DateTime.Today.Year + Year).ToString()),
            new LocalizedParam((startAge + Year).ToString()),
            new LocalizedParam(choicePathDictionary[Path], true));
    }

    /// <summary>
    /// When "stop" button is clicked, stop and reset time progression.
    /// </summary>
    public void TimeStop() {
        if (isTimePlaying) {
            TimePlayPause();
        }
        UpdateYear(0);
        sliderInteract.SetSlider(0);
        if (MasterManager.Instance.currPhase == GamePhase.VisPrius) {
            PriusManager.Instance.SetExplanationText();
        }
    }

    /// <summary>
    /// Helper method to progress through time. Currently updates on a year to year basis.
    /// </summary>
    IEnumerator TimeProgress() {
        while (YearValue <= 20) {
            // update on a yearly basis
            if (Mathf.RoundToInt(YearValue) != Year) {
                UpdateYear(YearValue);
            }

            sliderInteract.SetSlider(YearValue / 20);

            yield return null;
            YearValue += Time.deltaTime;
        }
        // after loop, stop.
        isTimePlaying = false;
        playPauseButton.ChangeImage(isTimePlaying);
        YearValue = 20;
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void Reset() {
        Path = HealthChoice.None;
        TimeStop();
    }
}
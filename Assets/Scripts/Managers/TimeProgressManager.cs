using System;
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

    // Tutorial-related variables
    [SerializeField] private Transform timeTutorialTransform;
    private bool updated;
    private bool stopped;

    private LongTermHealth ArchetypeHealth => ArchetypeManager.Instance.Performer.ArchetypeHealth;

    public float Index { get; private set; }

    private int Interval => NumMonths(ArchetypeHealth[0].date, ArchetypeHealth[1].date);

    private static int NumMonths(DateTime dt1, DateTime dt2) {
        return dt2.Month - dt1.Month + (dt2.Year - dt1.Year) * 12;
    }

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
    public void UpdateYear(float index) {
        updated = true;

        Index = index;
        UpdateHeaderText(Index);
        ArchetypeManager.Instance.displayer.UpdateStats(Index);

        foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
            performer.UpdateVisualization(Index);
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

    public void Cycle(bool on) {
        Playing = on;
        if (on) {
            // stopped/paused, start
            timeProgressCoroutine = CycleTime();
            StartCoroutine(timeProgressCoroutine);
        } else {
            // started, pause
            StopCoroutine(timeProgressCoroutine);
            TimeStop();
        }
    }

    /// <summary>
    /// Update header text.
    /// </summary>
    public void UpdateHeaderText(float index) {
        sliderText.text = Mathf.RoundToInt(index).ToString();

        DateTime dt = ArchetypeHealth[Mathf.FloorToInt(index)].date.AddMonths(Mathf.FloorToInt(Interval * (index % 1)));
        int yearsSinceStart = Mathf.FloorToInt(Interval * index / 12);
        headerText.SetText("Legends.HeaderText", new LocalizedParam(dt.Year), new LocalizedParam(dt.Month),
            new LocalizedParam(ArchetypeManager.Instance.displayer.ArchetypeData.age + yearsSinceStart));
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
    }

    /// <summary>
    /// Helper method to progress through time. Currently updates on a year to year basis.
    /// </summary>
    private IEnumerator TimeProgress() {
        Index = 0;
        while (Index < ArchetypeHealth.Count - 1) {
            // update on a yearly basis
            UpdateYear(Index);

            sliderInteract.SetSlider(Index / (ArchetypeHealth.Count - 1));

            yield return null;
            Index += Time.deltaTime;
        }

        // after loop, stop.
        Playing = false;
        UpdateYear(ArchetypeHealth.Count - 1);
    }

    private IEnumerator CycleTime() {
        while (true) {
            yield return TimeProgress();
            yield return new WaitForSeconds(5);
        }
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
                foreach (ArchetypePerformer performer in ArchetypeManager.Instance.performers) {
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
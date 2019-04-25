using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TimeProgressManager : MonoBehaviour {
    public static TimeProgressManager Instance { get; private set; }

    public PlayPauseButton playPauseButton;
    public Text headerText;
    public SliderInteract sliderInteract;
    public Text sliderText;

    private bool isTimePlaying;
    private IEnumerator timeProgressCoroutine;

    public HealthChoice Path { get; private set; }
    public int YearCount { get; set; }

    //TODO: replace placeholder age with real age
    public int startAge = 20;

    public readonly Dictionary<HealthChoice, string> choicePathDictionary = new Dictionary<HealthChoice, string> {
        {HealthChoice.None, "No Life Plan Change"},
        {HealthChoice.Minimal, "Minimal Change"},
        {HealthChoice.Recommended, "Optimal Change"}
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
    public void UpdateYear(int value) {
        YearCount = value;
        UpdateHeaderText();

        if (MasterManager.Instance.CurrGamePhase == GamePhase.VisActivity) {
            ActivityManager.Instance.Visualize(YearCount / 5, Path);
        } else if (MasterManager.Instance.CurrGamePhase == GamePhase.VisPrius) {
            bool healthChange = PriusManager.Instance.Visualize(YearCount / 5, Path);
            if (healthChange) {
                PriusManager.Instance.SetExplanationText();
                if (isTimePlaying) {
                    TimePlayPause();
                    TutorialText.Instance.ShowDouble("Health of oragns has changed", "Click on the panel to learn more", 3);
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
        TutorialText.Instance.Show("Switched to " + choicePathDictionary[Path], 3);

        if (MasterManager.Instance.CurrGamePhase == GamePhase.VisActivity) {
            ActivityManager.Instance.Visualize(YearCount / 5, Path);
        } else if (MasterManager.Instance.CurrGamePhase == GamePhase.VisPrius) {
            PriusManager.Instance.Visualize(YearCount / 5, Path);
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
        StringBuilder sliderBuilder = new StringBuilder(YearCount + " year");
        if (YearCount > 1) {
            sliderBuilder.Append("s");
        }
        sliderBuilder.Append(" from now");
        sliderText.text = sliderBuilder.ToString();

        StringBuilder headerBuilder = new StringBuilder("Year ");
        System.DateTime today = System.DateTime.Today;
        headerBuilder.Append(today.Year + YearCount);
        headerBuilder.Append(" (Age ");
        headerBuilder.Append(startAge + YearCount); // TODO: replace with actual age
        headerBuilder.Append(", " + choicePathDictionary[Path] + ")");

        headerText.text = headerBuilder.ToString();
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
        if (MasterManager.Instance.CurrGamePhase == GamePhase.VisPrius) {
            PriusManager.Instance.SetExplanationText();
        }
    }

    /// <summary>
    /// Helper method to progress through time.
    /// </summary>
    IEnumerator TimeProgress() {
        while (YearCount <= 20) {
            UpdateYear(YearCount);
            sliderInteract.SetSlider(((float)YearCount) / 20);

            yield return new WaitForSeconds(2f);
            YearCount += 5;
        }
        // after loop, stop.
        isTimePlaying = false;
        playPauseButton.ChangeImage(isTimePlaying);
        YearCount = 20; // reset year to 25.
        yield return null;
    }

    /// <summary>
    /// Jumps to a specific year. Added range checks.
    /// Used for "backward" and "forward" buttons on the control panel.
    /// NOT BEING USED NOW SINCE THE BUTTONS ARE HIDDEN.
    /// </summary>
    /// <param name="yearInterval">a year interval, NOT an actual year. For example, 5 means "5 years later".</param>
    public void TimeJump(int yearInterval) {
        int newYear;
        if (yearInterval > 0) {
            newYear = YearCount + yearInterval > 20 ? 20 : YearCount + yearInterval;
        } else {
            newYear = YearCount + yearInterval < 0 ? 0 : YearCount + yearInterval;
        }

        UpdateYear(newYear);
        TutorialText.Instance.Show("Switched to Year " + YearCount, 2);
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void Reset() {
        Path = HealthChoice.None;
        TimeStop();
    }
}
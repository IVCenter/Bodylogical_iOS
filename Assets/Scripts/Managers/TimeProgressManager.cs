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
    private int Year { get; set; }


    public readonly Dictionary<HealthChoice, string> choicePathDictionary = new Dictionary<HealthChoice, string> {
        {HealthChoice.None, "No Life Plan Change"},
        {HealthChoice.Minimal, "Minimal Change"},
        {HealthChoice.Recommended, "Optimal Change"}
    };

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
        Year = value;
        UpdateHeaderText();

        if (Path != HealthChoice.NotSet) {
            if (MasterManager.Instance.CurrGamePhase == GamePhase.VisActivity) {
                ActivityManager.Instance.Visualize(Year / 5, Path);
            } else if (MasterManager.Instance.CurrGamePhase == GamePhase.VisPrius) {
                bool healthChange = PriusManager.Instance.Visualize(Year / 5, Path);
                if (healthChange) {
                    if (isTimePlaying) {
                        TimePlayPause();
                        PriusManager.Instance.SetExplanationText();
                        TutorialText.Instance.ShowDouble("Health of oragns is changed", "Click on the panel to learn more", 3);
                    }
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
        if (Path != HealthChoice.NotSet) {
            TutorialText.Instance.Show("Switched to " + choicePathDictionary[Path], 3);

            if (MasterManager.Instance.CurrGamePhase == GamePhase.VisActivity) {
                ActivityManager.Instance.Visualize(Year / 5, Path);
            } else if (MasterManager.Instance.CurrGamePhase == GamePhase.VisPrius) {
                PriusManager.Instance.Visualize(Year / 5, Path);
                PriusManager.Instance.SetExplanationText();
            }
        }
    }

    /// <summary>
    /// When "play/pause" button is clicked, start/stop time progression.
    /// </summary>
    public void TimePlayPause() {
        if (Path == HealthChoice.NotSet) {
            TutorialText.Instance.ShowDouble("You haven't chosen a path", "Choose a path then continue", 3.0f);
        } else {
            if (!isTimePlaying) { // stopped/paused, start
                timeProgressCoroutine = TimeProgress();
                StartCoroutine(timeProgressCoroutine);
            } else { // started, pause
                StopCoroutine(timeProgressCoroutine);
            }
            isTimePlaying = !isTimePlaying;
            playPauseButton.ChangeImage(isTimePlaying);
        }
    }

    /// <summary>
    /// Update header text.
    /// </summary>
    public void UpdateHeaderText() {
        StringBuilder builder = new StringBuilder(Year + " year");
        if (Year > 1) {
            builder.Append("s");
        }
        sliderText.text = builder.ToString();

        if (Path != HealthChoice.NotSet) {
            builder.Append(" Later (" + choicePathDictionary[Path] + ")");
        }

        headerText.text = builder.ToString();
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
        while (Year <= 20) {
            UpdateYear(Year);
            sliderInteract.SetSlider(((float)Year) / 20);

            yield return new WaitForSeconds(2f);
            Year += 5;
        }
        // after loop, stop.
        isTimePlaying = false;
        playPauseButton.ChangeImage(isTimePlaying);
        Year = 20; // reset year to 25.
        // Animations should NOT be paused so that users can get closer view.
        //if (MasterManager.Instance.CurrGamePhase == GamePhase.VisActivity) { // pause animations
        //    ActivityManager.Instance.PauseAnimations();
        //}
        yield return null;
    }

    /// <summary>
    /// Jumps to a specific year. Added range checks.
    /// Used for "backward" and "forward" buttons on the control panel.
    /// </summary>
    /// <param name="yearInterval">a year interval, NOT an actual year. For example, 5 means "5 years later".</param>
    public void TimeJump(int yearInterval) {
        if (Path == HealthChoice.NotSet) {
            TutorialText.Instance.ShowDouble("You haven't chosen a path", "Choose a path then continue", 3.0f);
        } else {
            int newYear;
            if (yearInterval > 0) {
                newYear = Year + yearInterval > 25 ? 25 : Year + yearInterval;
            } else {
                newYear = Year + yearInterval < 0 ? 0 : Year + yearInterval;
            }

            UpdateYear(newYear);
            TutorialText.Instance.Show("Switched to Year " + Year, 2);
        }
    }

    /// <summary>
    /// Reset every visualization.
    /// </summary>
    public void Reset() {
        TimeStop();
        UpdatePath("NotSet");
    }
}

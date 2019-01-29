using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class AnimationManager : MonoBehaviour {
    public static AnimationManager Instance { get; private set; }

    public GameObject roomVisualization;
    public GameObject animationObjects;

    private bool isLeft;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Since each profile has his/her own model, the human model in the room must be
    /// dynamically generated.
    /// </summary>
    public void GenerateModelsForAnimations() {
        foreach (GameObject modelParent in roomVisualization.GetComponent<RoomVisualizer>().humanModels) {
            GameObject human = Instantiate(HumanManager.Instance.SelectedArchetype.modelPrefab);
            human.transform.SetParent(modelParent.transform, false);
        }
    }

    /// <summary>
    /// Switch to Animations view.
    /// </summary>
    public IEnumerator StartAnimations() {
        if (StageManager.Instance.Path == null) {
            yield return HumanManager.Instance.MoveSelectedHumanToCenter();
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use buttons to move through time", 3);
        } else {
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
            Visualize();
        }
        yield return null;
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void ToggleAnimation(bool on) {
        ButtonSequenceManager.Instance.SetPropsButton(!on);

        //roomVisualization.SetActive(on);
        animationObjects.SetActive(false); // only appears after clicking "play"
        ButtonSequenceManager.Instance.SetTimeSlider(on);
        ButtonSequenceManager.Instance.SetPathButtons(on);

        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetInternals(on);
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize() {
        //RoomVisualizer visualizer = roomVisualization.GetComponent<RoomVisualizer>();
        //visualizer.UpdateHeader(StageManager.Instance.Year, StageManager.Instance.Path);
        //visualizer.Visualize(GetPoint());
        if (!isLeft) {
            HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        animationObjects.SetActive(true);
    }
    
    /// <summary>
    /// Hardcoded point generated.
    /// TODO: to be removed by calculated data sets.
    /// </summary>
    /// <returns>The point.</returns>
    private int GetPoint() {
        string path = StageManager.Instance.Path;
        int year = StageManager.Instance.Year;
        if (path.Contains("No")) { // bad
            if (year < 5) {
                return 6;
            } else if (year < 10) {
                return 5;
            } else if (year < 15) {
                return 3;
            } else {
                return 1;
            }
        } else if (path.Contains("Minimal")) { // intermediate
            if (year < 5) {
                return 6;
            } else if (year < 10) {
                return 4;
            } else if (year < 15) {
                return 6;
            } else {
                return 7;
            }
        } else if (path.Contains("Optimal")) { // good
            if (year < 5) {
                return 6;
            } else if (year < 10) {
                return 5;
            } else if (year < 15) {
                return 7;
            } else {
                return 10;
            }
        } else {
            return 0; // default value
        }
    }
}

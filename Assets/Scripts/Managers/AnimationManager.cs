using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class AnimationManager : MonoBehaviour {
    public static AnimationManager Instance { get; private set; }

    public GameObject roomVisualization;

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
    /// Circulate through three props.
    /// </summary>
    public void ToggleAnimations() {
        if (StageManager.Instance.Path == null) {
            roomVisualization.SetActive(false);
            HumanManager.Instance.SelectedHuman.SetActive(false);
            ButtonSequenceManager.Instance.SetToggleButtons(false);
            ButtonSequenceManager.Instance.SetFunctionButtons(false);
            // need to switch back to line chart and internals easily
            ButtonSequenceManager.Instance.SetLineChartButton(true);
            ButtonSequenceManager.Instance.SetInternals(true);
            // need to hide props button and show sliders/paths buttons
            ButtonSequenceManager.Instance.SetPropsButton(false);
            ButtonSequenceManager.Instance.SetTimeSlider(true);
            ButtonSequenceManager.Instance.SetPathButtons(true);
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use slider to move through time", 3);
        } else { // a path is chosen. Show the room, 
            RoomVisualizer visualizer = roomVisualization.GetComponent<RoomVisualizer>();
            visualizer.UpdateHeader(StageManager.Instance.Year, StageManager.Instance.Path);
            visualizer.Visualize(GetPoint());
        }
    }

    /// <summary>
    /// Toggle a <b>single</b> prop. Does <b>NOT</b> change iterator.
    /// </summary>
    /// <param name="on">If set to <c>true</c> on.</param>
    public void ToggleAnimation(bool on) {
        roomVisualization.SetActive(on);
        ButtonSequenceManager.Instance.SetTimeSlider(on);
        ButtonSequenceManager.Instance.SetPathButtons(on);
    }

    public void Visualize(string keyword) {
        roomVisualization.SetActive(true);
        roomVisualization.GetComponent<RoomVisualizer>().Visualize(GetPoint());
        TutorialText.Instance.Show("Switched to " + keyword, 3);
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

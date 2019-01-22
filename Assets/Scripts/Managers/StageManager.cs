using System.Collections;
using UnityEngine;
using Collections.Hybrid.Generic;

public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }


    public GameObject stage;
    public GameObject stageObject;
    public GameObject controlPanel;
    public GameObject roomVisualization;
    public Transform[] positionList;


    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private Vector3 cp_initial_localPos;
    private bool isAnimating;

    private string path;
    private int year;

    #region Unity routines
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        cp_initial_localPos = new Vector3(controlPanel.transform.localPosition.x, controlPanel.transform.localPosition.y, controlPanel.transform.localPosition.z);
        isAnimating = false;
    }


    void Start() {
        posAvailableMap = new LinkedListDictionary<Transform, bool>();

        foreach (Transform trans in positionList) {
            posAvailableMap.Add(trans, true);
        }

        futureBlue = stageObject.GetComponent<MeshRenderer>().material.color;
        colorWhite = new Color(0, 1, 1, 0.42f);

        DisableControlPanel();
        DisableStage();
    }
    #endregion

    #region Stage control
    /// <summary>
    /// For each of the archetypes, create a model.
    /// </summary>
    public void BuildStage() {
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            human.CreateModel();
        }
    }

    public void EnableControlPanel() {
        if (!isAnimating) {
            StartCoroutine(FadeUpCP());
        }
    }

    IEnumerator FadeUpCP() {
        controlPanel.SetActive(true);

        float animation_time = 2.5f;
        float time_passed = 0;
        isAnimating = true;

        controlPanel.transform.localPosition = new Vector3(controlPanel.transform.localPosition.x, -10f, controlPanel.transform.localPosition.z);

        while (time_passed < animation_time) {
            controlPanel.transform.localPosition = Vector3.Lerp(controlPanel.transform.localPosition, cp_initial_localPos, 0.03f);

            time_passed += Time.deltaTime;
            yield return null;
        }

        controlPanel.transform.localPosition = cp_initial_localPos;

        isAnimating = false;

        yield return null;
    }


    public Transform GetAvailablePosInWorld() {
        foreach (Transform trans in positionList) {
            if (posAvailableMap[trans]) {
                posAvailableMap[trans] = false;
                return trans;
            }
        }
        return null;
    }

    public void UpdateStageTransform() {
        if (!stage.activeSelf) {
            stage.SetActive(true);
        }

        if (!stageObject.GetComponent<MeshRenderer>().enabled) {
            stageObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (CursorManager.Instance.cursor.FocusedObj != null) {
            GameObject obj = CursorManager.Instance.cursor.FocusedObj;

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>() != null) {
                Vector3 cursorPos = CursorManager.Instance.cursor.CursorPosition;
                Vector3 stageCenter = stage.transform.GetChild(0).position;
                Vector3 diff = stage.transform.position - stageCenter;
                stage.transform.position = cursorPos + diff;
                AdjustStageRotation(PlaneManager.Instance.MainPlane);
            }
        }

        Color lerpedColor = Color.Lerp(colorWhite, futureBlue, Mathf.PingPong(Time.time, 1));

        stageObject.GetComponent<MeshRenderer>().material.color = lerpedColor;
    }

    public void DisableStage() {
        stage.SetActive(false);
    }

    public void SettleStage() {
        stageObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void DisableControlPanel() {
        controlPanel.SetActive(false);
    }

    private void AdjustStageRotation(GameObject plane) {
        stage.transform.rotation = plane.transform.rotation;

        while (Vector3.Dot((stage.transform.position - Camera.main.transform.position), stage.transform.forward) < 0) {
            stage.transform.Rotate(0, 90, 0);
        }
    }
    #endregion

    #region PropsControl
    /// <summary>
    /// Since each profile has his/her own model, the human model in the room must be
    /// dynamically generated.
    /// </summary>
    public void GenerateModelsForProps() {
        foreach (GameObject modelParent in roomVisualization.GetComponent<RoomVisualizer>().humanModels) {
            GameObject human = Instantiate(HumanManager.Instance.SelectedArchetype.modelPrefab);
            human.transform.SetParent(modelParent.transform, false);
        }
    }

    /// <summary>
    /// Circulate through three props.
    /// </summary>
    public void ToggleProps(int year = 0) {
        this.year = year;
        if (path == null) {
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
            visualizer.UpdateHeader(year, path);
            visualizer.Visualize(GetPoint());
        }
    }

    /// <summary>
    /// Toggle a <b>single</b> prop. Does <b>NOT</b> change iterator.
    /// </summary>
    /// <param name="on">If set to <c>true</c> on.</param>
    public void ToggleProp(bool on) {
        roomVisualization.SetActive(on);
        ButtonSequenceManager.Instance.SetTimeSlider(on);
        ButtonSequenceManager.Instance.SetPathButtons(on);
    }

    public void TogglePath(string keyword) {
        path = keyword;
        roomVisualization.SetActive(true);
        roomVisualization.GetComponent<RoomVisualizer>().Visualize(GetPoint());
        TutorialText.Instance.Show("Switched to " + keyword, 3);
    }

    /// <summary>
    /// Hardcoded point generated.
    /// TODO: to be removed by calculated data sets.
    /// </summary>
    /// <returns>The point.</returns>
    public int GetPoint() {
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
    /// <summary>
    /// Resets the props.
    /// </summary>
    public void ResetProps() {
        year = 0;
        path = null;
    }
    #endregion

    #region Internals
    public void ToggleInternals() {
        ToggleProp(false);
        ResetProps();
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
        if (path == null) {
            TutorialText.Instance.ShowDouble("First click the path to visualize", "Then use slider to move through time", 3);
        } // if a path is chosen show the internals
            TutorialText.Instance.ShowDouble("You have entered internals visualization", "Placeholder, nothing here", 3);
    }
    #endregion
}
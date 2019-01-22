﻿using System.Collections;
using UnityEngine;
using Collections.Hybrid.Generic;

public class StageManager : MonoBehaviour {

    public static StageManager Instance;

    public GameObject stage;
    public GameObject controlPanel;
    public GameObject[] props;

    public Transform[] positionList;

    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private Vector3 cp_initial_localPos;
    private bool isAnimating;

    private int propsIterator = 0;

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

        futureBlue = stage.transform.GetComponent<MeshRenderer>().material.color;
        colorWhite = new Color(0, 1, 1, 0.42f);

        DisableControlPanel();
        DisableStage();
    }
    #endregion

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

    /// <summary>
    /// Circulate through three props.
    /// </summary>
    public void ToggleProps() {
        if (propsIterator == 0) {
            HumanManager.Instance.SelectedHuman.SetActive(false);
            ButtonSequenceManager.Instance.SetToggleButtons(false);
            ButtonSequenceManager.Instance.SetFunctionButtons(false);
            // need to switch back to line chart easily
            ButtonSequenceManager.Instance.SetLineChartButton(true);

            TutorialText.Instance.ShowDouble("Keep clicking \"Props\" to switch between model types", "Click 3 times or click \"Line Chart\" to go back", 5.0f);
        } else {
            ToggleProp(false);
        }

        propsIterator += 1;

        if (propsIterator > 3) {
            propsIterator = 0;

            HumanManager.Instance.SelectedHuman.SetActive(true);
            ButtonSequenceManager.Instance.SetToggleButtons(true);
            ButtonSequenceManager.Instance.SetFunctionButtons(true);

            // back to line chart, hide button
            ButtonSequenceManager.Instance.SetLineChartButton(false);

            TutorialText.Instance.Show("Now you are back to the chart visualization.", 3.8f);
        } else {
            ToggleProp(true);
        }
    }

    /// <summary>
    /// Toggle a <b>single</b> prop. Does <b>NOT</b> change iterator.
    /// </summary>
    /// <param name="on">If set to <c>true</c> on.</param>
    public void ToggleProp(bool on) {
        props[propsIterator - 1].SetActive(on);
    }

    /// <summary>
    /// Resets the props.
    /// </summary>
    public void ResetProps() {
        propsIterator = 0;
    }

    public void UpdateStageTransform() {
        if (!stage.activeSelf) {
            stage.SetActive(true);
        }

        if (!stage.GetComponent<MeshRenderer>().enabled) {
            stage.GetComponent<MeshRenderer>().enabled = true;
        }

        if (CursorManager.Instance.cursor.GetCurrentFocusedObj() != null) {
            GameObject obj = CursorManager.Instance.cursor.GetCurrentFocusedObj();

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>() != null) {
                Vector3 cursorPos = CursorManager.Instance.cursor.GetCursorPosition();
                Vector3 stageCenter = stage.transform.GetChild(0).position;
                Vector3 diff = stage.transform.position - stageCenter;
                stage.transform.position = cursorPos + diff;
                AdjustStageRotation(PlaneManager.Instance.MainPlane);
            }
        }

        Color lerpedColor = Color.Lerp(colorWhite, futureBlue, Mathf.PingPong(Time.time, 1));

        stage.GetComponent<MeshRenderer>().material.color = lerpedColor;
    }

    public void DisableStage() {
        stage.SetActive(false);
    }

    public void SettleStage() {
        stage.GetComponent<MeshRenderer>().enabled = false;
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
}

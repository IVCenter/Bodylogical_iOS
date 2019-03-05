using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ButtonSequenceManager : MonoBehaviour {
    public static ButtonSequenceManager Instance { get; private set; }

    [Header("Controls")]
    public GameObject predict;
    public GameObject info;
    public GameObject reset;

    [Header("Visualizations")]
    public GameObject lineChart;
    public GameObject activities;
    public GameObject prius;

    [Header("Functions")]
    public GameObject timeControls;
    public GameObject lineChartFunctions;
    public GameObject activityFunctions;
    public GameObject priusFunctions;



    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void InitializeButtons() {
        SetResetButton(true); // the use should be able to reset whenever the control panel is visible
        SetPredictButton(false);
        SetInfoButton(false);

        SetActivitiesButton(false);
        SetLineChartButton(false);
        SetPriusButton(false);

        SetLineChartFunction(false);
        SetActivityFunction(false);
        SetPriusFunction(false);
        SetTimeControls(false);
    }

    #region Controls
    public void SetPredictButton(bool isOn) {
        predict.SetActive(isOn);
    }

    public void SetResetButton(bool isOn) {
        reset.SetActive(isOn);
    }

    public void SetInfoButton(bool isOn) {
        info.SetActive(isOn);
    }
    #endregion

    #region Visualizations
    public void SetLineChartButton(bool isOn) {
        lineChart.SetActive(isOn);
    }

    public void SetActivitiesButton(bool isOn) {
        activities.SetActive(isOn);
    }

    public void SetPriusButton(bool isOn) {
        prius.SetActive(isOn);
    }
    #endregion

    #region Functions
    public void SetLineChartFunction(bool isOn) {
        lineChartFunctions.SetActive(isOn);
    }

    public void SetActivityFunction(bool isOn) {
        activityFunctions.SetActive(isOn);
    }

    public void SetPriusFunction(bool isOn) {
        priusFunctions.SetActive(isOn);
    }

    public void SetTimeControls(bool isOn) {
        timeControls.SetActive(isOn);
    }
    #endregion
}

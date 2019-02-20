using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ButtonSequenceManager : MonoBehaviour {
    public static ButtonSequenceManager Instance { get; private set; }

    public GameObject predict;
    public GameObject lineChart;
    public GameObject props;
    public GameObject reset;

    public GameObject timeSlider;
    public GameObject internals;

    public GameObject[] toggleButtons;
    public GameObject[] functionButtons;
    public GameObject[] pathButtons;


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
        SetPropsButton(false);
        SetLineChartButton(false);
        SetToggleButtons(false);
        SetFunctionButtons(false);
        SetPropsButton(false);
        SetTimeSlider(false);
        SetInternals(false);
        SetPathButtons(false);
    }

    public void SetPredictButton(bool isOn) {
        predict.SetActive(isOn);
    }

    public void SetLineChartButton(bool isOn) {
        lineChart.SetActive(isOn);
    }

    public void SetPropsButton(bool isOn) {
        props.SetActive(isOn);
    }

    public void SetResetButton(bool isOn) {
        reset.SetActive(isOn);
    }

    public void SetToggleButtons(bool isOn) {
        foreach (GameObject ggg in toggleButtons) {
            ggg.SetActive(isOn);
        }
    }

    public void SetFunctionButtons(bool isOn) {
        foreach (GameObject fff in functionButtons) {
            fff.SetActive(isOn);
        }
    }

    public void SetTimeSlider(bool isOn) {
        timeSlider.SetActive(isOn);
    }

    public void SetInternals(bool isOn) {
        internals.SetActive(isOn);
    }

    public void SetPathButtons(bool isOn) {
        foreach (GameObject eee in pathButtons) {
            eee.SetActive(isOn);
        }
    }
}

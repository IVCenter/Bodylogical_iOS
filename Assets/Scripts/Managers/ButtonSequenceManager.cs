using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ButtonSequenceManager : MonoBehaviour {
    public static ButtonSequenceManager Instance { get; private set; }

    public GameObject Predict;
    public GameObject LineChart;
    public GameObject Props;
    public GameObject Reset;

    public GameObject[] ToggleButtons;
    public GameObject[] FunctionButtons;



    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void InitializeButtonsActive() {
        SetPropsButton(true);
        SetResetButton(false);
        SetLineChartButton(false);
        SetToggleButtons(false);
        SetFunctionButtons(false);
        SetPropsButton(false);
    }

    public void SetPredictButton(bool isOn) {
        Predict.SetActive(isOn);
    }

    public void SetLineChartButton(bool isOn) {
        LineChart.SetActive(isOn);
    }

    public void SetPropsButton(bool isOn) {
        Props.SetActive(isOn);
    }

    public void SetResetButton(bool isOn) {
        Reset.SetActive(isOn);
    }

    public void SetToggleButtons(bool isOn) {
        foreach (GameObject ggg in ToggleButtons) {
            ggg.SetActive(isOn);
        }
    }

    public void SetFunctionButtons(bool isOn) {
        foreach (GameObject fff in FunctionButtons) {
            fff.SetActive(isOn);
        }
    }
}

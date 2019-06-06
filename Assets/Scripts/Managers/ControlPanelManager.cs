using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a utility class that provide API to enable and disable some of the buttons
/// So that we can make sure some button shows up before the other
/// </summary>
public class ControlPanelManager : MonoBehaviour {
    public static ControlPanelManager Instance { get; private set; }

    public GameObject predictPanel;
    public GameObject lineChartControls;
    public GameObject timeControls;

    public GameObject activitySelector;
    public GameObject priusSelector;
    public GameObject lineChartSelector;


    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void InitializeButtons() {
        TogglePredictPanel(false);
        ToggleLineChartControls(false);
        ToggleTimeControls(false);
        ToggleActivitySelector(false);
        TogglePriusSelector(false);
        ToggleLineChartSelector(false);
    }

    public void TogglePredictPanel(bool on) {
        predictPanel.SetActive(on);
    }

    public void ToggleLineChartControls(bool on) {
        lineChartControls.SetActive(on);
    }

    public void ToggleTimeControls(bool on) {
        timeControls.SetActive(on);
    }

    public void ToggleActivitySelector(bool on) {
        activitySelector.SetActive(on);
    }

    public void TogglePriusSelector(bool on) {
        priusSelector.SetActive(on);
    }
    public void ToggleLineChartSelector(bool on) {
        lineChartSelector.SetActive(on);
    }
}

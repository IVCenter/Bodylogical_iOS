using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailPanelManager : MonoBehaviour {
    public static DetailPanelManager Instance { get; private set; }

    public GameObject detailPanelObject;
    public PanelItem sleep, bp, bmi, bodyFat, calories;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues() {
        Lifestyle lifestyle = HumanManager.Instance.SelectedArchetype.ModelLifestyle;

        sleep.SetValue(lifestyle.sleepHours);
        bp.SetValue(lifestyle.sbp, 0);
        bp.SetValue(lifestyle.dbp, 1);
        bmi.SetValue(lifestyle.bmi);
        bodyFat.SetValue(lifestyle.bodyFat);
        calories.SetValue(lifestyle.calories);
    }

    public void ToggleDetailPanel(bool on) {
        detailPanelObject.SetActive(on);
    }
}

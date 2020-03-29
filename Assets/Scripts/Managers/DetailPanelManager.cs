using System.Collections.Generic;
using UnityEngine;

public class DetailPanelManager : MonoBehaviour {
    public static DetailPanelManager Instance { get; private set; }

    public GameObject detailPanelObject;
    public LocalizedText headerText;
    public PanelItem sleep, calories, exercise;
    public LocalizedText adherence;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues() {
        Lifestyle lifestyle = ArchetypeManager.Instance.selectedArchetype.lifestyleDict[HealthChoice.None];
        headerText.SetText("Archetypes.CurrentYear", new LocalizedParam(System.DateTime.Today.Year));
        sleep.SetValue(lifestyle.sleepHours);
        calories.SetValue(lifestyle.calories);
        exercise.SetValue(lifestyle.exercise);
        adherence.SetText(LocalizationDicts.statuses[lifestyle.adherence]);
    }

    public void ToggleDetailPanel(bool on) {
        detailPanelObject.SetActive(on);
    }
}

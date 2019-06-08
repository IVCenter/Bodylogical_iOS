using System.Collections.Generic;
using UnityEngine;

public class DetailPanelManager : MonoBehaviour {
    public static DetailPanelManager Instance { get; private set; }

    public GameObject detailPanelObject;
    public LocalizedText headerText;
    public PanelItem sleep, calories, exercise;
    public LocalizedText adherence;

    private readonly Dictionary<Adherence, string> adherences = new Dictionary<Adherence, string> {
        { Adherence.Bad, "Archetypes.PresBad" },
        { Adherence.Medium, "Archetypes.Medium" },
        { Adherence.Good, "Archetypes.Good" }
    };

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues() {
        Lifestyle lifestyle = HumanManager.Instance.SelectedArchetype.lifestyleDict[HealthChoice.None];
        headerText.SetText("Archetypes.CurrentYear", new LocalizedParam(System.DateTime.Today.Year.ToString()));
        sleep.SetValue(lifestyle.sleepHours);
        calories.SetValue(lifestyle.calories);
        exercise.SetValue(lifestyle.exercise);
        adherence.SetText(adherences[lifestyle.adherence]);
    }

    public void ToggleDetailPanel(bool on) {
        detailPanelObject.SetActive(on);
    }
}

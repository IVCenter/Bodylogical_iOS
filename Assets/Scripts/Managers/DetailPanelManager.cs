using UnityEngine;

public class DetailPanelManager : MonoBehaviour {
    public static DetailPanelManager Instance { get; private set; }

    [SerializeField] private GameObject detailPanelObject;
    [SerializeField] private LocalizedText headerText;
    [SerializeField] private PanelItem sleep, calories, exercise;
    [SerializeField] private LocalizedText adherence;

    private bool[] panelOpened = new bool[4];
    
    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues() {
        Lifestyle lifestyle = ArchetypeManager.Instance.Selected.ArchetypeData.lifestyleDict[HealthChoice.None];
        headerText.SetText("Archetypes.CurrentYear", new LocalizedParam(System.DateTime.Today.Year));
        sleep.SetValue(lifestyle.sleepHours);
        calories.SetValue(lifestyle.calories);
        exercise.SetValue(lifestyle.exercise);
        adherence.SetText(LocalizationDicts.statuses[lifestyle.adherence]);
    }

    public void ToggleDetailPanel(bool on) {
        detailPanelObject.SetActive(on);
    }

    /// <summary>
    /// Monitors the number of expandable panels that has been opened.
    /// </summary>
    public void OnPanelClick(int index) {
        panelOpened[index] = true;
        foreach (bool opened in panelOpened) {
            if (!opened) {
                return;
            }
        }
        
        // All four panels are opened, show icon
        ArchetypeManager.Instance.Selected.Icon.Initialize();
    }
}

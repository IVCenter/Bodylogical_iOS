using UnityEngine;

public class DetailPanel : MonoBehaviour {
    [SerializeField] private LocalizedText headerText;
    [SerializeField] private PanelItem sleep, diet, exercise;
    [SerializeField] private LocalizedText adherence;

    private bool[] panelOpened = new bool[4];
    
    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues() {
        ToggleDetailPanel(true);
        // Use the lifestyle for no intervention
        Lifestyle lifestyle = ArchetypeManager.Instance.Performers[HealthChoice.None].ArchetypeLifestyle;
        headerText.SetText("Archetypes.CurrentYear", new LocalizedParam(System.DateTime.Today.Year));
        sleep.SetValue(lifestyle.sleepHours);
        diet.SetValue(lifestyle.calories);
        exercise.SetValue(lifestyle.exercise);
        adherence.SetText(LocalizationDicts.statuses[lifestyle.adherence]);
    }

    public void ToggleDetailPanel(bool on) {
        gameObject.SetActive(on);
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
        ArchetypeManager.Instance.Selected.Icon.SetActive(true);
    }
}

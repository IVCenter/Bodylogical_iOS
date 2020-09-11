using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Although the "choice panel" is only displayed in the Ribon chart scene,
/// we might want it to show up in other visualizations, so use a separate manager here.
/// </summary>
public class ChoicePanelManager : MonoBehaviour {
    public static ChoicePanelManager Instance { get; private set; }

    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Image background;
    [SerializeField] private LocalizedText title;
    [SerializeField] private LocalizedText message;
    [SerializeField] private LocalizedText data;
    [SerializeField] private ColorLibrary colorLibrary;
    [SerializeField] 
    [Range(0, 255)]
    private int panelAlpha;

    private readonly Dictionary<HealthChoice, string> texts = new Dictionary<HealthChoice, string> {
        { HealthChoice.None, "Legends.InfoCurrentTitle" },
        { HealthChoice.Minimal, "Legends.InfoMinimalTitle" },
        { HealthChoice.Optimal, "Legends.InfoOptimalTitle" }
    };

    private readonly Dictionary<HealthChoice, string> messages = new Dictionary<HealthChoice, string> {
        { HealthChoice.None, "Legends.InfoCurrent" },
        { HealthChoice.Minimal, "Legends.InfoMinimal" },
        { HealthChoice.Optimal, "Legends.InfoOptimal" }
    };

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void ToggleChoicePanels(bool on) {
        choicePanel.SetActive(on);
    }

    public void SetValues() {
        HealthChoice choice = TimeProgressManager.Instance.Path;
        Color color = colorLibrary.ChoiceColorDict[choice];
        color.a = panelAlpha / 255f;
        background.color = color;
        title.SetText(texts[choice]);
        message.SetText(messages[choice]);

        Lifestyle lifestyle = ArchetypeManager.Instance.Selected.ArchetypeData.lifestyleDict[choice];
        data.SetText("Legends.InfoTemplate",
            new LocalizedParam(lifestyle.sleepHours),
            new LocalizedParam(lifestyle.exercise),
            new LocalizedParam(lifestyle.calories),
            new LocalizedParam(LocalizationDicts.statuses[lifestyle.adherence], true)
        );
    }
}

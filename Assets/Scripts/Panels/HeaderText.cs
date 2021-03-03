using UnityEngine;

public class HeaderText : MonoBehaviour {
    [SerializeField] private LocalizedText text;

    private LocalizedParam nameParam;
    private const string InfoTemplate = "Legends.HeaderInfo";
    private const string MeetTemplate = "Legends.HeaderMeet";
    private const string JourneyTemplate = "Legends.HeaderJourney";

    /// <summary>
    /// This must be called first before SetMeet() and SetJourney().
    /// </summary>
    /// <param name="archetype"></param>
    public void SetInfo(Archetype archetype) {
        nameParam = new LocalizedParam(archetype.Name, true);
        text.SetText(InfoTemplate, nameParam,
            new LocalizedParam(archetype.age),
            new LocalizedParam(archetype.Occupation, true),
            new LocalizedParam(LocalizationDicts.statuses[archetype.status], true));
    }

    public void SetMeet() {
        text.SetText(MeetTemplate, nameParam);
    }

    public void SetJourney() {
        text.SetText(JourneyTemplate, nameParam);
    }
}
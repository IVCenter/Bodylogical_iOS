using UnityEngine;

public class InfoPanel : MonoBehaviour {
    public LocalizedText name, age, occupation, disease;

    public void Initialize(Archetype archetype) {
        name.SetText("Archetypes.Name", new LocalizedParam(archetype.Name, true));
        age.SetText("Archetypes.Age", new LocalizedParam(archetype.age));
        occupation.SetText("Archetypes.Occupation", new LocalizedParam(archetype.Occupation, true));
        disease.SetText("Archetypes.Status", new LocalizedParam(LocalizationDicts.statuses[archetype.status], true));
    }

    public void SetActive(bool on) {
        gameObject.SetActive(on);
    }
}
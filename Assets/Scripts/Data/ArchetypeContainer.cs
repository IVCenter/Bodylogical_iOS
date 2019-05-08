using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A container for preset archetypes.
/// </summary>
public class ArchetypeContainer : MonoBehaviour {

    public static ArchetypeContainer Instance { get; private set; }

    /// <summary>
    /// The profiles with data.
    /// </summary>
    public List<Archetype> profiles;

    public GameObject modelTemplate;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    void Start() {
        TextAsset archetypes = Resources.Load<TextAsset>("Data/Archetypes");
        profiles = CSVParser.LoadCsv<Archetype>(archetypes.text);

        TextAsset lifestyle = Resources.Load<TextAsset>("Data/P1Lifestyle");
        List<Lifestyle> lifestyles = CSVParser.LoadCsv<Lifestyle>(lifestyle.text);
        foreach (Archetype archetype in profiles) {
            archetype.lifestyleDict = lifestyles.ToDictionary(x => x.choice, x => x);
        }
    }
}

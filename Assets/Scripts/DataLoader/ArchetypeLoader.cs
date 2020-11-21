using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A container for preset archetypes.
/// </summary>
public class ArchetypeLoader : MonoBehaviour {
    public static ArchetypeLoader Instance { get; private set; }

    /// <summary>
    /// The profiles with data.
    /// </summary>
    public List<Archetype> Profiles { get; private set; }

    public GameObject modelTemplate;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Loads all archetype-related data.
    /// </summary>
    private void Start() {
        TextAsset archetypes = Resources.Load<TextAsset>("Data/Archetypes");
        Profiles = CSVParser.LoadCsv<Archetype>(archetypes.text);
        
        foreach (Archetype archetype in Profiles) {
            // Load lifestyle
            TextAsset lifestyle = Resources.Load<TextAsset>($"Data/P{archetype.id}Lifestyle");
            List<Lifestyle> lifestyles = CSVParser.LoadCsv<Lifestyle>(lifestyle.text);
            archetype.lifestyleDict = lifestyles.ToDictionary(x => x.choice, x => x);
            
            // Load health data
            archetype.healthDict = new Dictionary<HealthChoice, LongTermHealth>();
            foreach (HealthChoice choice in Enum.GetValues(typeof(HealthChoice)).Cast<HealthChoice>()) {
                TextAsset asset = Resources.Load<TextAsset>($"Data/P{archetype.id}{choice}");
                List<Health> health = CSVParser.LoadCsv<Health>(asset.text);
                archetype.healthDict[choice] = new LongTermHealth(health);
            }
        }
    }
}

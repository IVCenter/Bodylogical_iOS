using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Loads all data from the csv files.
/// </summary>
public static class DataLoader {
    public static List<Archetype> LoadArchetypes() {
        TextAsset archetypes = Resources.Load<TextAsset>("Data/Archetypes");
        return CSVParser.LoadCsv<Archetype>(archetypes.text);
    }

    public static Dictionary<HealthChoice, Lifestyle> LoadLifestyles(Archetype archetype) {
        TextAsset lifestyle = Resources.Load<TextAsset>($"Data/P{archetype.id}Lifestyle");
        List<Lifestyle> lifestyles = CSVParser.LoadCsv<Lifestyle>(lifestyle.text);
        return lifestyles.ToDictionary(x => x.choice, x => x);
    }

    public static Dictionary<HealthChoice, LongTermHealth> LoadHealthData(Archetype archetype) {
        Dictionary<HealthChoice, LongTermHealth> healthData = new Dictionary<HealthChoice, LongTermHealth>();
        foreach (HealthChoice choice in Enum.GetValues(typeof(HealthChoice)).Cast<HealthChoice>()) {
            TextAsset asset = Resources.Load<TextAsset>($"Data/P{archetype.id}{choice}");
            List<Health> health = CSVParser.LoadCsv<Health>(asset.text);
            healthData[choice] = new LongTermHealth(health);
        }

        return healthData;
    }

    public static List<HealthRange> LoadRanges() {
        TextAsset rangeAsset = Resources.Load<TextAsset>("Data/Ranges");
        return CSVParser.LoadCsv<HealthRange>(rangeAsset.text);
    }
}
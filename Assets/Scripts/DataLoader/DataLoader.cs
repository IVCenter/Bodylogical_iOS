using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads data from the csv files.
/// </summary>
public static class DataLoader {
    public static List<Lifestyle> LoadLifestyles() {
        TextAsset lifestyleAsset = Resources.Load<TextAsset>("Data/Lifestyles");
        return CSVParser.LoadCsv<Lifestyle>(lifestyleAsset.text);
    }
    
    public static List<HealthRange> LoadRanges() {
        TextAsset rangeAsset = Resources.Load<TextAsset>("Data/Ranges");
        return CSVParser.LoadCsv<HealthRange>(rangeAsset.text);
    }
}
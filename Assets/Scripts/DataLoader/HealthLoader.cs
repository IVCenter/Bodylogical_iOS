using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: refactor when more data comes. (probably a tree structure?)
/// </summary>
public class HealthLoader : MonoBehaviour {
    public static HealthLoader Instance { get; private set; }

    public Dictionary<HealthChoice, LongTermHealth> ChoiceDataDictionary { get; private set; }

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// TODO: more health data might be added later.
    /// This would cause more than one health data corresponding to one health status.
    /// </summary>
    void Start() {
        ChoiceDataDictionary = new Dictionary<HealthChoice, LongTermHealth>();

        TextAsset noChangeAsset = Resources.Load<TextAsset>("Data/P1None");
        List<Health> noChangeHealths = CSVParser.LoadCsv<Health>(noChangeAsset.text);
        ChoiceDataDictionary[HealthChoice.None] = new LongTermHealth(noChangeHealths);

        TextAsset minimalAsset = Resources.Load<TextAsset>("Data/P1Minimal");
        List<Health> minimalHealths = CSVParser.LoadCsv<Health>(minimalAsset.text);
        ChoiceDataDictionary[HealthChoice.Minimal] = new LongTermHealth(minimalHealths);

        TextAsset optimalAsset = Resources.Load<TextAsset>("Data/P1Optimal");
        List<Health> optimalHealths = CSVParser.LoadCsv<Health>(optimalAsset.text);
        ChoiceDataDictionary[HealthChoice.Optimal] = new LongTermHealth(optimalHealths);
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RangeLoader : MonoBehaviour {
    public static RangeLoader Instance { get; private set; }

    private List<HealthRange> ranges;
    
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// populates the dictionary.
    /// </summary>
    void Start() {
        TextAsset rangeAsset = Resources.Load<TextAsset>("Data/Ranges");
        ranges = CSVParser.LoadCsv<HealthRange>(rangeAsset.text);
    }

    public HealthRange GetRange(HealthType type, Gender gender) {
        var selectedRanges = from r in ranges
                             where r.type == type && (r.gender == gender || r.gender == Gender.Either)
                             select r;

        return selectedRanges.First();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiometricContainer : MonoBehaviour {
    public static BiometricContainer Instance { get; private set; }

    public Dictionary<HealthType, HealthRange> StatusRangeDictionary { get; private set; }

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
        StatusRangeDictionary = new Dictionary<HealthType, HealthRange>();
        foreach (Transform child in transform) {
            HealthRange range = child.GetComponent<HealthRange>();
            StatusRangeDictionary.Add(range.type, range);
        }
    }
}

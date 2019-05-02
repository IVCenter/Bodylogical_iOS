using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: refactor when more data comes. (probably a tree structure?)
/// </summary>
public class HealthDataContainer : MonoBehaviour {
    public static HealthDataContainer Instance { get; private set; }

    public Dictionary<HealthChoice, LongTermHealth> choiceDataDictionary;

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
        choiceDataDictionary = new Dictionary<HealthChoice, LongTermHealth>();
        foreach (Transform child in transform) {
            LongTermHealth health = child.GetComponent<LongTermHealth>();
            choiceDataDictionary.Add(health.profileChoice, health);
        }
    }
}

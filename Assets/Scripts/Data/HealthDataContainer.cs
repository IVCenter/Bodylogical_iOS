using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDataContainer : MonoBehaviour {
    public static HealthDataContainer Instance { get; private set; }

    public Dictionary<HealthStatus, LongTermHealth> statusDataDictionary;

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
        statusDataDictionary = new Dictionary<HealthStatus, LongTermHealth>();
        foreach (Transform child in transform) {
            LongTermHealth health = child.GetComponent<LongTermHealth>();
            statusDataDictionary.Add(health.profileStatus, health);
        }
    }
}

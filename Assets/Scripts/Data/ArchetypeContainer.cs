using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A container for preset archetypes.
/// </summary>
public class ArchetypeContainer : MonoBehaviour {

    public static ArchetypeContainer Instance { get; private set; }

    /// <summary>
    /// The profiles with data.
    /// </summary>
    public Archetype[] profiles;

    public GameObject modelTemplate;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
}

using System.Collections.Generic;

/// <summary>
/// Contains the avatar's basic information, included in Archetypes.csv.
/// </summary>
[System.Serializable]
public class Archetype {
    public int id;
    public Gender gender;
    public int age;
    public HealthStatus status;
    public string modelString;

    public Dictionary<HealthChoice, Lifestyle> lifestyleDict;

    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Name => $"Archetypes.P{id}Name";
    /// <summary>
    /// Localized key entry for occupation.
    /// </summary>
    public string Occupation => $"Archetypes.P{id}Occupation";
}

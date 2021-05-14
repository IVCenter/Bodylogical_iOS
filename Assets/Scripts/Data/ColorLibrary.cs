using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Library", menuName = "Bodylogical/Color Library", order = 1)]
public class ColorLibrary : ScriptableObject {
    [Header("Interventions")] public Color noChangeColor;
    public Color worseColor;
    public Color betterColor;
    [Header("Health Status")] public Color badColor;
    public Color intermediateColor;
    public Color goodColor;

    public Dictionary<HealthChoice, Color> ChoiceColorDict { get; private set; }
    public Dictionary<HealthStatus, Color> StatusColorDict { get; private set; }

    private void OnEnable() {
        if (ChoiceColorDict == null) {
            ChoiceColorDict = new Dictionary<HealthChoice, Color> {
                {HealthChoice.Custom, noChangeColor},
                {HealthChoice.Worse, worseColor},
                {HealthChoice.Better, betterColor}
            };
        }

        if (StatusColorDict == null) {
            StatusColorDict = new Dictionary<HealthStatus, Color> {
                {HealthStatus.Bad, badColor},
                {HealthStatus.Moderate, intermediateColor},
                {HealthStatus.Good, goodColor}
            };
        }
    }
}
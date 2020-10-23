using System.Collections.Generic;
using UnityEngine;

public static class LiverHealth {
    public static int score;
    public static HealthStatus status;

    private static readonly Dictionary<HealthStatus, string> messages
        = new Dictionary<HealthStatus, string> {
        { HealthStatus.Good, "Legends.PriLiverGood" },
        { HealthStatus.Moderate, "Legends.PriLiverIntermediate" },
        { HealthStatus.Bad, "Legends.PriLiverBad" }
    };

    public static string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);

    /// <summary>
    /// Liver is related to bmi and ldl values. Use these two to generate a new status.
    /// </summary>
    /// <returns>true if the status has changed since the last call, false otherwise.</returns>
    public static bool UpdateStatus(float index, HealthChoice choice) {
        Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        LongTermHealth health = data.healthDict[choice];
        score = health.CalculateHealth(index, data.gender, HealthType.bmi, HealthType.ldl);
        HealthStatus currStatus = HealthUtil.CalculateStatus(score);

        // Floats are inaccurate; equals index == 0
        if (Mathf.Abs(index) <= 0.001f) {
            status = currStatus;
            return false;
        }

        bool changed = currStatus != status;
        status = currStatus;

        return changed;
    }
}

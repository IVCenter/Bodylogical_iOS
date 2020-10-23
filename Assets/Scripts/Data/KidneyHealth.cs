using System.Collections.Generic;
using UnityEngine;

public static class KidneyHealth {
    public static int score;
    public static HealthStatus status;

    private static readonly Dictionary<HealthStatus, string> messages
        = new Dictionary<HealthStatus, string> {
        { HealthStatus.Good, "Legends.PriKidneyGood" },
        { HealthStatus.Moderate, "Legends.PriKidneyIntermediate" },
        { HealthStatus.Bad, "Legends.PriKidneyBad" }
    };

    public static string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);
    
    /// <summary>
    /// Kidney is related to sbp and aic values. Use these two to generate a new status.
    /// </summary>
    /// <returns>true if the status has changed since the last call, false otherwise.</returns>
    public static bool UpdateStatus(float index, HealthChoice choice) {
        Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        LongTermHealth health = data.healthDict[choice];
        score = health.CalculateHealth(index, data.gender, HealthType.sbp, HealthType.aic);
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

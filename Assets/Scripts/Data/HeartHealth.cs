using System.Collections.Generic;
using UnityEngine;

public static class HeartHealth {
    public static int score;
    public static HealthStatus status;

    private static readonly Dictionary<HealthStatus, string> messages
        = new Dictionary<HealthStatus, string> {
            {HealthStatus.Good, "Legends.PriHeartGood"},
            {HealthStatus.Moderate, "Legends.PriHeartIntermediate"},
            {HealthStatus.Bad, "Legends.PriHeartBad"}
        };

    public static string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);

    /// <summary>
    /// Health is related to sbp and ldl values. Use these two to generate a new status.
    /// </summary>
    /// <returns>true if the status has changed since the last call, false otherwise.</returns>
    public static bool UpdateStatus(float index, HealthChoice choice) {
        Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        LongTermHealth health = data.healthDict[choice];
        int flooredIndex =  Mathf.FloorToInt(index);
        int ceiledIndex = Mathf.CeilToInt(index);

        float sbpValue = Mathf.Lerp(health.Sbp[flooredIndex], health.Sbp[ceiledIndex], index % 1);
        int sbpScore = HealthUtil.CalculatePoint(HealthType.sbp, data.gender, sbpValue);

        float ldlValue = Mathf.Lerp(health.Ldl[flooredIndex], health.Ldl[ceiledIndex], index % 1);
        int ldlScore = HealthUtil.CalculatePoint(HealthType.ldl, data.gender, ldlValue);

        score = (sbpScore + ldlScore) / 2;
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
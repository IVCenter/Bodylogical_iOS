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

    public static bool UpdateStatus(float index, HealthChoice choice) {
        Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        LongTermHealth health = data.healthDict[choice];
        
        float sbpValue = Mathf.Lerp(health.Sbp[(int)Mathf.Floor(index)],
            health.Sbp[(int)Mathf.Ceil(index)],
            index % 1);
        int sbpScore = HealthUtil.CalculatePoint(HealthType.sbp,
            data.gender,
            sbpValue);

        float aicValue = Mathf.Lerp(health.Aic[(int)Mathf.Floor(index)],
            health.Aic[(int)Mathf.Ceil(index)],
            index % 1);
        int aicScore = HealthUtil.CalculatePoint(HealthType.aic, data.gender, aicValue);

        score = (sbpScore + aicScore) / 2;
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

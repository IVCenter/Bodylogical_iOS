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

    public static bool UpdateStatus(float index, HealthChoice choice) {
        Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        LongTermHealth health = data.healthDict[choice];
        
        float bmiValue = Mathf.Lerp(health.Bmi[(int)Mathf.Floor(index)],
            health.Bmi[(int)Mathf.Ceil(index)],
            index % 1);
        int bmiScore = HealthUtil.CalculatePoint(HealthType.bmi, data.gender, bmiValue);

        float ldlValue = Mathf.Lerp(health.Ldl[(int)Mathf.Floor(index)],
            health.Ldl[(int)Mathf.Ceil(index)],
            index % 1);
        int ldlScore = HealthUtil.CalculatePoint(HealthType.ldl, data.gender, ldlValue);

        score = (bmiScore + ldlScore) / 2;
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

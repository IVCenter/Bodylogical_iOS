using System.Collections.Generic;
using UnityEngine;

public static class HeartHealth {
    public static int score;
    public static HealthStatus status;

    private static readonly Dictionary<HealthStatus, string> messages
        = new Dictionary<HealthStatus, string> {
        { HealthStatus.Good, "Legends.PriHeartGood" },
        { HealthStatus.Moderate, "Legends.PriHeartIntermediate" },
        { HealthStatus.Bad, "Legends.PriHeartBad" }
    };

    public static string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);

    public static bool UpdateStatus(float index, HealthChoice choice) {
        float sbpValue = Mathf.Lerp(
            HealthLoader.Instance.choiceDataDictionary[choice].sbp[(int)Mathf.Floor(index)],
            HealthLoader.Instance.choiceDataDictionary[choice].sbp[(int)Mathf.Ceil(index)],
            index % 1);
        int sbpScore = RangeLoader.Instance.CalculatePoint(HealthType.sbp,
            ArchetypeManager.Instance.Selected.archetype.gender,
            sbpValue);

        float ldlValue = Mathf.Lerp(
            HealthLoader.Instance.choiceDataDictionary[choice].ldl[(int)Mathf.Floor(index)],
            HealthLoader.Instance.choiceDataDictionary[choice].ldl[(int)Mathf.Ceil(index)],
            index % 1);
        int ldlScore = RangeLoader.Instance.CalculatePoint(HealthType.ldl,
            ArchetypeManager.Instance.Selected.archetype.gender,
            ldlValue);

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

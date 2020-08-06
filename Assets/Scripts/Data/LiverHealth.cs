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
        float bmiValue = Mathf.Lerp(
            HealthLoader.Instance.ChoiceDataDictionary[choice].Bmi[(int)Mathf.Floor(index)],
            HealthLoader.Instance.ChoiceDataDictionary[choice].Bmi[(int)Mathf.Ceil(index)],
            index % 1);
        int bmiScore = RangeLoader.Instance.CalculatePoint(HealthType.bmi,
            ArchetypeManager.Instance.Selected.ArchetypeData.gender,
            bmiValue);

        float ldlValue = Mathf.Lerp(
            HealthLoader.Instance.ChoiceDataDictionary[choice].Ldl[(int)Mathf.Floor(index)],
            HealthLoader.Instance.ChoiceDataDictionary[choice].Ldl[(int)Mathf.Ceil(index)],
            index % 1);
        int ldlScore = RangeLoader.Instance.CalculatePoint(HealthType.ldl,
            ArchetypeManager.Instance.Selected.ArchetypeData.gender,
            ldlValue);

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

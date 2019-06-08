using System.Collections.Generic;
using UnityEngine;

public static class LiverHealth {
    public static int score;
    public static HealthStatus status;
    public static string connectionMsg = "Legends.PriLiverGeneral";

    public static PriusType Type { get { return PriusType.Heart; } }

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private static readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "Legends.PriLiverGoodVerbose" },
                { false, "Legends.PriLiverGoodConcise" }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
                { true, "Legends.PriLiverIntermediateVerbose" },
                { false, "Legends.PriLiverIntermediateConcise" }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "Legends.PriLiverBadVerbose" },
                { false, "Legends.PriLiverBadConcise" }
            }
        }
    };

    public static string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Liver;
            if (expand) {
                return LocalizationManager.Instance.FormatString(connectionMsg)
                    + "\n" + LocalizationManager.Instance.FormatString(messages[status][true]);
            }
            return LocalizationManager.Instance.FormatString(messages[status][false]);
        }
    }


    public static bool UpdateStatus(float index, HealthChoice choice) {
        float bmiValue = Mathf.Lerp(
            HealthDataContainer.Instance.choiceDataDictionary[choice].bmi[(int)Mathf.Floor(index)],
            HealthDataContainer.Instance.choiceDataDictionary[choice].bmi[(int)Mathf.Ceil(index)],
            index % 1);
        int bmiScore = BiometricContainer.Instance.CalculatePoint(HealthType.bmi,
            HumanManager.Instance.selectedArchetype.gender,
            bmiValue);

        float ldlValue = Mathf.Lerp(
            HealthDataContainer.Instance.choiceDataDictionary[choice].ldl[(int)Mathf.Floor(index)],
            HealthDataContainer.Instance.choiceDataDictionary[choice].ldl[(int)Mathf.Ceil(index)],
            index % 1);
        int ldlScore = BiometricContainer.Instance.CalculatePoint(HealthType.ldl,
            HumanManager.Instance.selectedArchetype.gender,
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

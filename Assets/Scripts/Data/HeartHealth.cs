using System.Collections.Generic;
using UnityEngine;

public static class HeartHealth {
    public static int score;
    public static HealthStatus status;
    public static string connectionMsg = "Legends.PriHeartGeneral";
    public static PriusType Type { get { return PriusType.Heart; } }

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private static readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "Legends.PriHeartGoodVerbose" },
                { false, "Legends.PriHeartGoodConcise" }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
                { true, "Legends.PriHeartIntermediateVerbose" },
                { false, "Legends.PriHeartIntermediateConcise." }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "Legends.PriHeartBadVerbose" },
                { false, "Legends.PriHeartBadConcise" }
            }
        }
    };

    public static string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Heart;
            if (expand) {
                return LocalizationManager.Instance.FormatString(connectionMsg)
                    + "\n" + LocalizationManager.Instance.FormatString(messages[status][true]);
            }
            return LocalizationManager.Instance.FormatString(messages[status][false]);
        }
    }

    public static bool UpdateStatus(float index, HealthChoice choice) {
        float sbpValue = Mathf.Lerp(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[(int)Mathf.Floor(index)],
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[(int)Mathf.Ceil(index)],
            index % 1);
        int sbpScore = BiometricContainer.Instance.CalculatePoint(HealthType.sbp,
            HumanManager.Instance.SelectedArchetype.gender,
            sbpValue);

        float ldlValue = Mathf.Lerp(
            HealthDataContainer.Instance.choiceDataDictionary[choice].ldl[(int)Mathf.Floor(index)],
            HealthDataContainer.Instance.choiceDataDictionary[choice].ldl[(int)Mathf.Ceil(index)],
            index % 1);
        int ldlScore = BiometricContainer.Instance.CalculatePoint(HealthType.ldl,
            HumanManager.Instance.SelectedArchetype.gender,
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

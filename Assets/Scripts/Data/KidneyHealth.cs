using System.Collections.Generic;
using UnityEngine;

public static class KidneyHealth {
    public static int score;
    public static HealthStatus status;
    public static string connectionMsg = "Legends.PriKidneyGeneral";
    public static PriusType Type => PriusType.Heart;

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private static readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "Legends.PriKidneyGoodVerbose" },
                { false, "Legends.PriKidneyGoodConcise" }
            }
        },{
            HealthStatus.Moderate, new Dictionary<bool, string> {
                { true, "Legends.PriKidneyIntermediateVerbose" },
                { false, "Legends.PriKidneyIntermediateConcise" }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "Legends.PriKidneyBadVerbose" },
                { false, "Legends.PriKidneyBadConcise" }
            }
        }
    };

    public static string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Kidney;
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
            HumanManager.Instance.selectedArchetype.gender,
            sbpValue);

        float aicValue = Mathf.Lerp(
            HealthDataContainer.Instance.choiceDataDictionary[choice].aic[(int)Mathf.Floor(index)],
            HealthDataContainer.Instance.choiceDataDictionary[choice].aic[(int)Mathf.Ceil(index)],
            index % 1);
        int aicScore = BiometricContainer.Instance.CalculatePoint(HealthType.aic,
            HumanManager.Instance.selectedArchetype.gender,
            aicValue);

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

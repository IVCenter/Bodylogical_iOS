using System.Collections.Generic;

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

    public static bool UpdateStatus(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.CalculatePoint(HealthType.sbp,
            HumanManager.Instance.SelectedArchetype.gender,
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int ldlScore = BiometricContainer.Instance.CalculatePoint(HealthType.ldl,
            HumanManager.Instance.SelectedArchetype.gender,
            HealthDataContainer.Instance.choiceDataDictionary[choice].ldl[index]);

        score = (sbpScore + ldlScore) / 2;
        HealthStatus currStatus = HealthUtil.CalculateStatus(score);

        if (index == 0) {
            status = currStatus;
            return false;
        }

        bool changed = currStatus != status;
        status = currStatus;

        return changed;
    }
}

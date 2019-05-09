using System.Collections.Generic;

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


    public static bool UpdateStatus(int index, HealthChoice choice) {
        int bmiScore = BiometricContainer.Instance.CalculatePoint(HealthType.bmi, HumanManager.Instance.SelectedArchetype.gender,
            HealthDataContainer.Instance.choiceDataDictionary[choice].bmi[index]);

        int ldlScore = BiometricContainer.Instance.CalculatePoint(HealthType.ldl, HumanManager.Instance.SelectedArchetype.gender,
            HealthDataContainer.Instance.choiceDataDictionary[choice].ldl[index]);

        score = (bmiScore + ldlScore) / 2;
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

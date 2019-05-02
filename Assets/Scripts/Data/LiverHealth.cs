using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiverHealth : MonoBehaviour {
    [HideInInspector]
    public int score;
    [HideInInspector]
    public HealthStatus status;
    public PriusType Type { get { return PriusType.Heart; } }
    [HideInInspector]
    public string connectionMsg = "Legends.PriLiverGeneral";

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
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
                { false, "Legends.PriLiverConcise" }
            }
        }
    };

    public string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Liver;
            if (expand) {
                return LocalizationManager.Instance.FormatString(connectionMsg)
                    + "\n" + LocalizationManager.Instance.FormatString(messages[status][true]);
            }
            return LocalizationManager.Instance.FormatString(messages[status][false]);
        }
    }


    public bool UpdateStatus(int index, HealthChoice choice) {
        int bmiScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.bmi].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].BMI[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);

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

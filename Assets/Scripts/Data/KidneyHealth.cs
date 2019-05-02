using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidneyHealth : MonoBehaviour {
    [HideInInspector]
    public int score;
    [HideInInspector]
    public HealthStatus status;
    public PriusType Type { get { return PriusType.Heart; } }
    [HideInInspector]
    public string connectionMsg = "Legends.PriKidneyGeneral";

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "Legends.PriKidneyGoodVerbose" },
                { false, "Legends.PriKidneyGoodConcise" }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
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

    public string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Kidney;
            if (expand) {
                return LocalizationManager.Instance.FormatString(connectionMsg)
                    + "\n" + LocalizationManager.Instance.FormatString(messages[status][true]);
            }
            return LocalizationManager.Instance.FormatString(messages[status][false]);
        }
    }

    public bool UpdateStatus(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int aicScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.aic].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].aic[index]);

        score = (sbpScore + aicScore) / 2;
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

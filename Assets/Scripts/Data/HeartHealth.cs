using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartHealth : MonoBehaviour {
    [HideInInspector]
    public int score;
    [HideInInspector]
    public HealthStatus status;
    public PriusType Type { get { return PriusType.Heart; } }
    [HideInInspector]
    public string connectionMsg = "Heart health is related to blood pressure and LDL.";

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "Low blood pressure and cholesterol levels mean a healthy circulatory system." },
                { false, "Heart is normal." }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
                { true, "Rising blood pressure and cholesterol will start damaging arteries that can lead to clogging." },
                { false, "Heart has trouble pumping blood." }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "High blood pressure and cholesterol will clog the blood pressure and cause problems sucha s stroke, heart attack, etc." },
                { false, "Arteries have high chance of clogging, potentials for heart attacks." }
            }
        }
    };

    public string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Heart;
            if (expand) {
                return connectionMsg + "\n" + messages[status][true];
            }
            return messages[status][false];
        }
    }

    public bool UpdateStatus(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);

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

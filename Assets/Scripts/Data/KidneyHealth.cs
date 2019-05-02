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
    public string connectionMsg = "Kindey health is related to blood pressure and glucose.";

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "With low blood pressure and low glucose allows the kidney to filter toxins out of the blood." },
                { false, "Kidney is normal." }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
                { true, "Rising blood pressure and glucose starts to impair kidney function to filter toxins." },
                { false, "Kidney is in process of diabetes." }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "Persistent high blood pressure and high glucosee levels have led to kidney damage and the kidney stops filtering toxins, causing diabetes." },
                { false, "Kidney is experiencing diabetes." }
            }
        }
    };

    public string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Kidney;
            if (expand) {
                return connectionMsg + "\n" + messages[status][true];
            }
            return messages[status][false];
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

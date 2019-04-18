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
    public string connectionMsg = "Liver health is related to BMI and LDL.";

    /// <summary>
    /// messages[status][true]: expanded message.
    /// messages[status][false]: brief message.
    /// </summary>
    private readonly Dictionary<HealthStatus, Dictionary<bool, string>> messages = new Dictionary<HealthStatus, Dictionary<bool, string>> {
        {
            HealthStatus.Good, new Dictionary<bool, string> {
                { true, "With a slim body and low LDL levels the liver has no problems." },
                { false, "Liver is normal." }
            }
        },{
            HealthStatus.Intermediate, new Dictionary<bool, string> {
                { true, "A larger waist and rising LDL means the liver is accumulating fat." },
                { false, "Liver is in the process of fatty liver." }
            }
        },{
            HealthStatus.Bad, new Dictionary<bool, string> {
                { true, "Obesity and persistent high LDL cause fat to accumulate in the liver and leads to fatty liver." },
                { false, "Fatty liver has formed." }
            }
        }
    };

    public string ExplanationText {
        get {
            bool expand = PriusManager.Instance.currentPart == PriusType.Liver;
            if (expand) {
                return connectionMsg + "\n" + messages[status][true];
            }
            return messages[status][false];
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

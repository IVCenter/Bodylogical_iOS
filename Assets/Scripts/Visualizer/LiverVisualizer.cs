using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiverVisualizer : Visualizer {
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    public GameObject goodLiver, badLiver;

    public override  HealthStatus Status { get; set; }

    public string connectionMsg = "Liver health is related to BMI and LDL.\n";

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
                return connectionMsg + messages[Status][true];
            }
            return messages[Status][false];
        }
    }

    /// <summary>
    /// Visualize the liver.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public override bool Visualize(int index, HealthChoice choice) {
        int bmiScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.bmi].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].BMI[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);

        HealthStatus currStatus = HealthUtil.CalculateStatus((bmiScore + ldlScore) / 2);

        if (index == 0) {
            Status = currStatus;
            return false;
        }

        bool changed = currStatus != Status;
        Status = currStatus;

        ShowOrgan();

        return changed;
    }

    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            if (Status == HealthStatus.Bad) {
                goodLiver.SetActive(false);
                badLiver.SetActive(true);
            } else {
                goodLiver.SetActive(true);
                badLiver.SetActive(false);
            }
        }
    }
}

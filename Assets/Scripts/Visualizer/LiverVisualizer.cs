using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiverVisualizer : MonoBehaviour {
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    public GameObject goodLiver, badLiver;

    public HealthStatus LiverStatus { get; private set; }

    public string ExplanationText {
        get {
            switch (LiverStatus) {
                case HealthStatus.Good:
                    return "Liver is normal.";
                case HealthStatus.Intermediate:
                    return "Kidney is in process of fatty liver.";
                case HealthStatus.Bad:
                    return "Fatty liver has formed.";
                default:
                    return "ERROR";
            }
        }
    }

    /// <summary>
    /// Visualize the liver.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public bool Visualize(int index, HealthChoice choice) {
        int bmiScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.bmi].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].BMI[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);

        HealthStatus currStatus = HealthUtil.CalculateStatus((bmiScore + ldlScore) / 2);

        if (index == 0) {
            LiverStatus = currStatus;
            return false;
        }

        bool changed = currStatus != LiverStatus;
        LiverStatus = currStatus;

        ShowOrgan();

        return changed;
    }

    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            if (LiverStatus == HealthStatus.Bad) {
                goodLiver.SetActive(false);
                badLiver.SetActive(true);
            } else {
                goodLiver.SetActive(true);
                badLiver.SetActive(false);
            }
        }
    }
}

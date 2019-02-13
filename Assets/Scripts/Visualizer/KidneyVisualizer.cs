using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidneyVisualizer : MonoBehaviour {
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    public GameObject goodKidney, badKidney;

    public HealthStatus KidneyStatus { get; private set; }

    public string ExplanationText {
        get {
            switch (KidneyStatus) {
                case HealthStatus.Good:
                    return "Kidney is normal.";
                case HealthStatus.Intermediate:
                    return "Kidney is in process of diabetes.";
                case HealthStatus.Bad:
                    return "Kidney is experiencing diabetes.";
                default:
                    return "ERROR";
            }
        }
    }

    /// <summary>
    /// Visualize the kidney.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public bool Visualize(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int aicScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.aic].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].aic[index]);

        HealthStatus currStatus = HealthUtil.CalculateStatus((sbpScore + aicScore) / 2);

        if (index == 0) {
            KidneyStatus = currStatus;
            return false;
        }

        bool changed = currStatus != KidneyStatus;
        KidneyStatus = currStatus;

        ShowOrgan();
        return changed;
    }

    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            if (KidneyStatus == HealthStatus.Bad) {
                goodKidney.SetActive(false);
                badKidney.SetActive(true);
            } else {
                goodKidney.SetActive(true);
                badKidney.SetActive(false);
            }
        }
    }
}

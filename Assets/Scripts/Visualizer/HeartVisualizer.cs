using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartVisualizer : MonoBehaviour {
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    //public GameObject goodKidney, badKidney;

    public HealthStatus HeartStatus { get; private set; }

    public string ExplanationText {
        get {
            switch (HeartStatus) {
                case HealthStatus.Good:
                    return "Heart is normal.";
                case HealthStatus.Intermediate:
                    return "Heart has trouble pumping blood.";
                case HealthStatus.Bad:
                    return "Arteries have clogged, possible of heart attacks.";
                default:
                    return "ERROR";
            }
        }
    }

    /// <summary>
    /// Visualize the heart.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public bool Visualize(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);


        HealthStatus currStatus = HealthUtil.CalculateStatus((sbpScore + ldlScore) / 2);

        if (index == 0) {
            HeartStatus = currStatus;
            return false;
        }

        //if (currStatus == HealthStatus.Bad) {
        //    goodKidney.SetActive(false);
        //    badKidney.SetActive(true);
        //} else {
        //    goodKidney.SetActive(true);
        //    badKidney.SetActive(false);
        //}

        bool changed = currStatus != HeartStatus;
        HeartStatus = currStatus;
        return changed;
    }
}

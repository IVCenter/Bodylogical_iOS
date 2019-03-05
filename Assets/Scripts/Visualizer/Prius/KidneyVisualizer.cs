using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidneyVisualizer : Visualizer {
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    public GameObject goodKidney, badKidney;

    public override HealthStatus Status { get; set; }

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
                return connectionMsg + "\n" + messages[Status][true];
            }
            return messages[Status][false];
        }
    }

    /// <summary>
    /// Visualize the kidney.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public override bool Visualize(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int aicScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.aic].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].aic[index]);

        HealthStatus currStatus = HealthUtil.CalculateStatus((sbpScore + aicScore) / 2);

        if (index == 0) {
            Status = currStatus;
            return false;
        }

        bool changed = currStatus != Status;
        Status = currStatus;

        ShowOrgan();
        return changed;
    }

    //TODO: change back
    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    if (Status == HealthStatus.Good) {
                        goodKidney.SetActive(true);
                        badKidney.SetActive(false);
                    } else {
                        goodKidney.SetActive(false);
                        badKidney.SetActive(true);
                    }
                    break;
                case PriusShowStatus.Good:
                    goodKidney.SetActive(true);
                    badKidney.SetActive(false);
                    break;
                case PriusShowStatus.Intermediate:
                case PriusShowStatus.Bad:
                    goodKidney.SetActive(false);
                    badKidney.SetActive(true);
                    break;
            }
        }
    }

    public override void Pause() {
        throw new System.NotImplementedException();
    }
}

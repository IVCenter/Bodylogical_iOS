﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidneyVisualizer : Visualizer {
    public override string VisualizerName { get { return "Kidney"; } }
    /// <summary>
    /// TODO: change to cross-section
    /// </summary>
    public GameObject goodLeftKidney, goodRightKidney, badLeftKidney, badRightKidney;
    [HideInInspector]
    public bool leftSelected;
    private GameObject CurrGood { get { return leftSelected ? goodLeftKidney : goodRightKidney; } }
    private GameObject OtherGood { get { return leftSelected ? goodRightKidney : goodLeftKidney; } }
    private GameObject CurrBad { get { return leftSelected ? badLeftKidney : badRightKidney; } }
    private GameObject OtherBad { get { return leftSelected ? goodRightKidney : goodLeftKidney; } }

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

    //TODO: animation
    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    if (Status == HealthStatus.Good) {
                        CurrGood.SetActive(true);
                        OtherGood.SetActive(false);
                        CurrBad.SetActive(false);
                        OtherBad.SetActive(false);
                    } else {
                        CurrGood.SetActive(false);
                        OtherGood.SetActive(false);
                        CurrBad.SetActive(true);
                        OtherBad.SetActive(false);
                    }
                    break;
                case PriusShowStatus.Good:
                    CurrGood.SetActive(true);
                    OtherGood.SetActive(false);
                    CurrBad.SetActive(false);
                    OtherBad.SetActive(false);
                    break;
                case PriusShowStatus.Intermediate:
                case PriusShowStatus.Bad:
                    CurrGood.SetActive(false);
                    OtherGood.SetActive(false);
                    CurrBad.SetActive(true);
                    OtherBad.SetActive(false);
                    break;
            }
        }
    }

    public override void Pause() {
        throw new System.NotImplementedException();
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartVisualizer : Visualizer {
    public override string VisualizerName { get { return "Heart"; } }

    public GameObject heart;
    private Animator HeartAnimator { get { return heart.transform.GetChild(0).GetComponent<Animator>(); } }
    private int healthScore;
    public override HealthStatus Status { get; set; }

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
                return connectionMsg + "\n" + messages[Status][true];
            }
            return messages[Status][false];
        }
    }

    /// <summary>
    /// Visualize the heart.
    /// </summary>
    /// <returns>true if it changes state, false otherwise.</returns>
    /// <param name="index">index, NOT the year.</param>
    public override bool Visualize(int index, HealthChoice choice) {
        int sbpScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.sbp].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].sbp[index]);

        int ldlScore = BiometricContainer.Instance.StatusRangeDictionary[HealthType.ldl].CalculatePoint(
            HealthDataContainer.Instance.choiceDataDictionary[choice].LDL[index]);

        healthScore = (sbpScore + ldlScore) / 2;
        HealthStatus currStatus = HealthUtil.CalculateStatus(healthScore);

        if (index == 0) {
            Status = currStatus;
            return false;
        }

        bool changed = currStatus != Status;
        Status = currStatus;

        ShowOrgan();
        return changed;
    }

    // TODO: smooth change?
    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            heart.SetActive(true);
            // calculate animation speed
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    HeartAnimator.speed = 1.0f - healthScore / 100.0f;
                    break;
                case PriusShowStatus.Bad:
                    HeartAnimator.speed = 1.0f;
                    break;
                case PriusShowStatus.Intermediate:
                    HeartAnimator.speed = 0.75f;
                    break;
                case PriusShowStatus.Good:
                    HeartAnimator.speed = 0.5f;
                    break;
            }
        }
    }

    public override void Pause() {
        throw new System.NotImplementedException();
    }
}

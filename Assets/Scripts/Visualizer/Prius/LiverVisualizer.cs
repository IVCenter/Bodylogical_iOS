using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiverVisualizer : Visualizer {
    public override string VisualizerName { get { return "Liver"; } }

    public GameObject liver;
    public Texture normalMap;
    private int healthScore;

    public override HealthStatus Status { get; set; }

    public string connectionMsg = "Liver health is related to BMI and LDL.";
    
    void Awake() {
        foreach (Transform obj in liver.transform) {
            Material mat = obj.GetComponent<MeshRenderer>().material;
            mat.EnableKeyword("_NORMALMAP");
            mat.EnableKeyword("_PARALLAXMAP");
        }
    }

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
                return connectionMsg + "\n" + messages[Status][true];
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

        healthScore = (bmiScore + ldlScore) / 2;

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

    // TODO: fluid change?
    public void ShowOrgan() {
        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    if (Status != HealthStatus.Good) {
                        // healthScore ranges from 0 - 60 when not in Good status.
                        // Converts it to 0-1 scale, and then lerp.
                        DisplayHeightMap(Mathf.Lerp(0.005f, 0.08f, healthScore / 60.0f));

                    } else {
                        HideHeightMap();
                    }
                    break;
                case PriusShowStatus.Bad:
                    DisplayHeightMap(0.08f);
                    break;
                case PriusShowStatus.Intermediate:
                    DisplayHeightMap(0.005f);
                    break;
                case PriusShowStatus.Good:
                    HideHeightMap();
                    break;
            }
        }
    }

    public override void Pause() {
        throw new System.NotImplementedException();
    }

    private void DisplayHeightMap(float value) {
        foreach (Transform obj in liver.transform) {
            Material mat = obj.GetComponent<MeshRenderer>().material;
            mat.SetTexture("_BumpMap", normalMap);
            mat.SetFloat("_Parallax", value);
        }
    }

    private void HideHeightMap() {
        foreach (Transform obj in liver.transform) {
            obj.GetComponent<MeshRenderer>().material.SetTexture("_BumpMap", null);
        }
    }
}

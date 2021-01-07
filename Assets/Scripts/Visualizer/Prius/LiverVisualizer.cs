using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LiverVisualizer : OrganVisualizer {
    [SerializeField] private GameObject liver;
    [SerializeField] private Image indicator;
    [SerializeField] private SlideBarPointer slidebar;

    private readonly Dictionary<HealthStatus, string> messages = new Dictionary<HealthStatus, string> {
        {HealthStatus.Good, "Legends.PriLiverGood"},
        {HealthStatus.Moderate, "Legends.PriLiverIntermediate"},
        {HealthStatus.Bad, "Legends.PriLiverBad"}
    };

    private SkinnedMeshRenderer LiverRenderer => liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
    public override string ExplanationText => LocalizationManager.Instance.FormatString(messages[status]);

    public override bool Visualize(float index, HealthChoice choice) {
        bool liverChanged = UpdateStatus(index, choice);
        //indicator.color = PriusManager.Instance.colorLibrary.StatusColorDict[status];

        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            LiverRenderer.SetBlendShapeWeight(0, 100 - score);
        }

        slidebar.SetProgress(score);
        return liverChanged;
    }

    /// <summary>
    /// Liver is related to bmi and ldl values. Use these two to generate a new status.
    /// </summary>
    /// <returns>true if the status has changed since the last call, false otherwise.</returns>
    public override bool UpdateStatus(float index, HealthChoice choice) {
        return false;
        // Archetype data = ArchetypeManager.Instance.Selected.ArchetypeData;
        // LongTermHealth health = data.healthDict[choice];
        // score = health.CalculateHealth(index, data.gender, HealthType.bmi, HealthType.ldl);
        // HealthStatus currStatus = HealthUtil.CalculateStatus(score);
        //
        // // Floats are inaccurate; equals index == 0
        // if (Mathf.Abs(index) <= 0.001f) {
        //     status = currStatus;
        //     return false;
        // }
        //
        // bool changed = currStatus != status;
        // status = currStatus;
        //
        // return changed;
    }
}
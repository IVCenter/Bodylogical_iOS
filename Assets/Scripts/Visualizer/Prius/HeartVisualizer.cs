using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartVisualizer : OrganVisualizer {
    [SerializeField] private GameObject heart;
    [SerializeField] private Image indicator;
    [SerializeField] private SlideBar slidebar;

    private readonly Dictionary<HealthStatus, string> messages = new Dictionary<HealthStatus, string> {
        {HealthStatus.Good, "Legends.PriHeartGood"},
        {HealthStatus.Moderate, "Legends.PriHeartIntermediate"},
        {HealthStatus.Bad, "Legends.PriHeartBad"}
    };

    private Animator HeartAnimator => heart.transform.GetChild(0).GetComponent<Animator>();

    public override bool Visualize(float index) {
        bool heartChanged = UpdateStatus(index);
        indicator.color = Library.StatusColorDict[status];

        if (gameObject.activeInHierarchy) {
            heart.SetActive(true);
            HeartAnimator.speed = 1.0f - score / 100.0f;
        }

        slidebar.SetProgress(score);
        return heartChanged;
    }

    /// <summary>
    /// Health is related to sbp and ldl values. Use these two to generate a new status.
    /// </summary>
    /// <returns>true if the status has changed since the last call, false otherwise.</returns>
    public virtual bool UpdateStatus(float index) {
        score = performer.ArchetypeHealth.CalculateHealth(index, performer.ArchetypeData.gender, HealthType.sbp,
            HealthType.ldl);
        HealthStatus currStatus = HealthUtil.CalculateStatus(score);

        // Floats are inaccurate; equals index == 0
        if (Mathf.Abs(index) <= 0.001f) {
            status = currStatus;
            return false;
        }

        bool changed = currStatus != status;
        status = currStatus;

        return changed;
    }
}
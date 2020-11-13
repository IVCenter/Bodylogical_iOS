using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An overall prius visualzer that controls the individual organ visualizers.
/// </summary>
public class PriusVisualizer : Visualizer {
    [SerializeField] private Color goodColor, intermediateColor, badColor;
    [SerializeField] private Image heartIndicator, liverIndicator, kidneyIndicator;
    [SerializeField] private OrganDisplay heart, liver, kidney;
    [SerializeField] private SlideBarPointer heartStatus, liverStatus, kidneyStatus;

    private HealthStatus status;

    /// <summary>
    /// Detailed text that would be shown on the panel.
    /// </summary>
    public string ExplanationText {
        get {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(HeartHealth.ExplanationText);
            builder.AppendLine(LiverHealth.ExplanationText);
            builder.AppendLine(KidneyHealth.ExplanationText);
            return builder.ToString();
        }
    }

    public override bool Visualize(float index, HealthChoice choice) {
        bool heartChanged = HeartHealth.UpdateStatus(index, choice);
        heartIndicator.color = UpdateColor(HeartHealth.status);
        heart.DisplayOrgan(HeartHealth.score);
        // Added for status display
        heartStatus.SetProgress(HeartHealth.score);

        bool kidneyChanged = KidneyHealth.UpdateStatus(index, choice);
        kidneyIndicator.color = UpdateColor(KidneyHealth.status);
        kidney.DisplayOrgan(KidneyHealth.score);
        kidneyStatus.SetProgress(KidneyHealth.score);

        bool liverChanged = LiverHealth.UpdateStatus(index, choice);
        liverIndicator.color = UpdateColor(LiverHealth.status);
        liver.DisplayOrgan(LiverHealth.score);
        liverStatus.SetProgress(LiverHealth.score);

        return heartChanged || kidneyChanged || liverChanged;
    }

    /// <summary>
    /// Generates the new color for the legend panels.
    /// </summary>
    /// <returns>The color.</returns>
    /// <param name="status">Status.</param>
    private Color UpdateColor(HealthStatus status) {
        if (status == HealthStatus.Bad) {
            return badColor;
        }

        if (status == HealthStatus.Moderate) {
            return intermediateColor;
        }

        return goodColor;
    }
}
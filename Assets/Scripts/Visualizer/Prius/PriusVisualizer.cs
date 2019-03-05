using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriusVisualizer : Visualizer {
    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;

    public HeartVisualizer heartVisualizer;
    public LiverVisualizer liverVisualizer;
    public KidneyVisualizer kidneyVisualizer;

    public override HealthStatus Status { get; set; }

    /// <summary>
    /// Detailed text that would be shown on the panel.
    /// </summary>
    public string ExplanationText {
        get {
            StringBuilder builder = new StringBuilder();
            switch (PriusManager.Instance.currentPart) {
                case PriusType.Human:
                    builder.AppendLine(heartVisualizer.ExplanationText);
                    builder.AppendLine(liverVisualizer.ExplanationText);
                    builder.Append(kidneyVisualizer.ExplanationText);
                    break;
                case PriusType.Heart:
                    builder.AppendLine(heartVisualizer.ExplanationText);
                    break;
                case PriusType.Kidney:
                    builder.Append(kidneyVisualizer.ExplanationText);
                    break;
                case PriusType.Liver:
                    builder.AppendLine(liverVisualizer.ExplanationText);
                    break;
                case PriusType.Pancreas:
                    break;
            }

            return builder.ToString();
        }
    }

    public override bool Visualize(int index, HealthChoice choice) {
        bool heartChanged = heartVisualizer.Visualize(index, choice);
        heartIndicator.color = SetColor(heartVisualizer.Status);

        bool kidneyChanged = kidneyVisualizer.Visualize(index, choice);
        kidneyIndicator.color = SetColor(kidneyVisualizer.Status);

        bool liverChanged = liverVisualizer.Visualize(index, choice);
        liverIndicator.color = SetColor(liverVisualizer.Status);

        return heartChanged || kidneyChanged || liverChanged;
    }

    private Color SetColor(HealthStatus status) {
        if (status == HealthStatus.Bad) {
            return badColor;
        }

        if (status == HealthStatus.Intermediate) {
            return intermediateColor;
        }

        return goodColor;
    }

    public void ShowOrgan(PriusType type) {
        switch (type) {
            case PriusType.Liver:
                liverVisualizer.ShowOrgan();
                break;
            case PriusType.Kidney:
                kidneyVisualizer.ShowOrgan();
                break;
            case PriusType.Heart:
                heartVisualizer.ShowOrgan();
                break;
        }
    }

    /// <summary>
    /// Reserved when the animations for prius is ready.
    /// </summary>
    public override void Pause() {
        throw new System.NotImplementedException();
    }
}
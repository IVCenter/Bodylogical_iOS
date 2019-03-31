using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An overall prius visualzer that controls the miniscule organs.
/// </summary>
public class PriusVisualizer : Visualizer {
    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;
    public GameObject goodHeart, badHeart, goodLiver, badLiver, goodKidney, badKidney;
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
        SetOrgan(heartVisualizer.Status, goodHeart, badHeart);

        bool kidneyChanged = kidneyVisualizer.Visualize(index, choice);
        kidneyIndicator.color = SetColor(kidneyVisualizer.Status);
        SetOrgan(kidneyVisualizer.Status, goodKidney, badKidney);

        bool liverChanged = liverVisualizer.Visualize(index, choice);
        liverIndicator.color = SetColor(liverVisualizer.Status);
        SetOrgan(liverVisualizer.Status, goodLiver, badLiver);

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

    /// <summary>
    /// TODO: This setting is for demo only. Need to set to HealthStatus.Bad (or have three models).
    /// </summary>
    /// <param name="status">Status.</param>
    /// <param name="good">Good.</param>
    /// <param name="bad">Bad.</param>
    private void SetOrgan(HealthStatus status, GameObject good, GameObject bad) {
        bad.SetActive(status != HealthStatus.Good);
        good.SetActive(status == HealthStatus.Good);
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
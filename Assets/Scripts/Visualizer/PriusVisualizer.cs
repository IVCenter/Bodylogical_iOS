using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriusVisualizer : MonoBehaviour {
    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;

    public HeartVisualizer heartVisualizer;
    public LiverVisualizer liverVisualizer;
    public KidneyVisualizer kidneyVisualizer;

    public string ExplanationText {
        get {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(heartVisualizer.ExplanationText);
            builder.AppendLine(liverVisualizer.ExplanationText);
            builder.Append(kidneyVisualizer.ExplanationText);
            return builder.ToString();
        }
    }

    public bool Visualize(int index, HealthChoice choice) {
        bool heartChanged = heartVisualizer.Visualize(index, choice);
        heartIndicator.color = SetColor(heartVisualizer.HeartStatus);

        bool kidneyChanged = kidneyVisualizer.Visualize(index, choice);
        kidneyIndicator.color = SetColor(kidneyVisualizer.KidneyStatus);

        bool liverChanged = liverVisualizer.Visualize(index, choice);
        liverIndicator.color = SetColor(liverVisualizer.LiverStatus);

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
        }
    }
}

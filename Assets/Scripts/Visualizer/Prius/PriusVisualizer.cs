using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An overall prius visualzer that controls the individual organ visualizers.
/// </summary>
public class PriusVisualizer : Visualizer {
    public override string VisualizerKey { get { return "Buttons.Prius"; } }

    public HeartHealth heartHealth;
    public LiverHealth liverHealth;
    public KidneyHealth kidneyHealth;

    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;

    public OrganDisplay smallHeart, largeHeart, smallLiver, largeLiver, smallKidney, largeKidney;

    public override HealthStatus Status { get; set; }

    /// <summary>
    /// Detailed text that would be shown on the panel.
    /// </summary>
    public string ExplanationText {
        get {
            StringBuilder builder = new StringBuilder();
            switch (PriusManager.Instance.currentPart) {
                case PriusType.Human:
                    builder.AppendLine(heartHealth.ExplanationText);
                    builder.AppendLine(liverHealth.ExplanationText);
                    builder.AppendLine(kidneyHealth.ExplanationText);
                    break;
                case PriusType.Heart:
                    builder.AppendLine(heartHealth.ExplanationText);
                    break;
                case PriusType.Kidney:
                    builder.Append(kidneyHealth.ExplanationText);
                    break;
                case PriusType.Liver:
                    builder.AppendLine(liverHealth.ExplanationText);
                    break;
                case PriusType.Pancreas:
                    break;
            }

            return builder.ToString();
        }
    }

    public override bool Visualize(int index, HealthChoice choice) {
        bool heartChanged = heartHealth.UpdateStatus(index, choice);
        heartIndicator.color = UpdateColor(heartHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Heart) {
            largeHeart.DisplayOrgan(heartHealth.score, heartHealth.status);
        } else {
            smallHeart.DisplayOrgan(heartHealth.score, heartHealth.status);
        }

        bool kidneyChanged = kidneyHealth.UpdateStatus(index, choice);
        kidneyIndicator.color = UpdateColor(kidneyHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Kidney) {
            largeKidney.DisplayOrgan(kidneyHealth.score, kidneyHealth.status);
        } else {
            smallKidney.DisplayOrgan(kidneyHealth.score, kidneyHealth.status);
        }

        bool liverChanged = liverHealth.UpdateStatus(index, choice);
        liverIndicator.color = UpdateColor(liverHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Liver) {
            largeLiver.DisplayOrgan(liverHealth.score, liverHealth.status);
        } else {
            smallLiver.DisplayOrgan(liverHealth.score, liverHealth.status);
        }

        return heartChanged || kidneyChanged || liverChanged;
    }

    private Color UpdateColor(HealthStatus status) {
        if (status == HealthStatus.Bad) {
            return badColor;
        }

        if (status == HealthStatus.Intermediate) {
            return intermediateColor;
        }

        return goodColor;
    }

    /// <summary>
    /// Moves the organ.
    /// </summary>
    /// <param name="stl">move small to large (true) or large to small (false)</param>
    /// <param name="type">Type.</param>
    /// <param name="small">Small.</param>
    /// <param name="large">Large.</param>
    public void MoveOrgan(bool stl, PriusType type, GameObject small, GameObject large) {
        if (stl) {
            StartCoroutine(MoveSmallToLarge(type, small, large));
        } else {
            StartCoroutine(MoveLargeToSmall(type, small, large));
        }
    }

    private IEnumerator MoveSmallToLarge(PriusType type, GameObject small, GameObject large) {
        Vector3 startPos = small.transform.position;
        Vector3 endPos = large.transform.position;

        Vector3 startScale = small.transform.localScale;
        Vector3 endScale = startScale * 5.0f;

        for (int i = 0; i < 100; i++) {
            Vector3 curr = Vector3.Lerp(startPos, endPos, (float)i / 100);
            small.transform.position = curr;
            small.transform.localScale = Vector3.Lerp(startScale, endScale, (float)i / 100);
            yield return null;
        }

        small.transform.position = startPos;
        small.transform.localScale = startScale;
        small.SetActive(false);
        large.SetActive(true);
        DisplayOrgan(type);
    }

    private IEnumerator MoveLargeToSmall(PriusType type, GameObject small, GameObject large) {
        Vector3 startPos = large.transform.position;
        Vector3 endPos = small.transform.position;

        Vector3 startScale = large.transform.localScale;
        Vector3 endScale = startScale * 0.2f;

        for (int i = 0; i < 100; i++) {
            Vector3 curr = Vector3.Lerp(startPos, endPos, (float)i / 100);
            large.transform.position = curr;
            large.transform.localScale = Vector3.Lerp(startScale, endScale, (float)i / 100);
            yield return null;
        }

        large.transform.position = startPos;
        large.transform.localScale = startScale;
        large.SetActive(false);
        small.SetActive(true);
    }

    public void DisplayOrgan(PriusType type) {
        switch (type) {
            case PriusType.Liver:
                largeLiver.DisplayOrgan();
                break;
            case PriusType.Kidney:
                largeKidney.DisplayOrgan();
                break;
            case PriusType.Heart:
                largeHeart.DisplayOrgan();
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
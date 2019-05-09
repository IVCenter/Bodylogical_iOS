using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An overall prius visualzer that controls the individual organ visualizers.
/// </summary>
public class PriusVisualizer : Visualizer {
    public override string VisualizerKey { get { return "Buttons.Prius"; } }

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
                    builder.AppendLine(HeartHealth.ExplanationText);
                    builder.AppendLine(LiverHealth.ExplanationText);
                    builder.AppendLine(KidneyHealth.ExplanationText);
                    break;
                case PriusType.Heart:
                    builder.AppendLine(HeartHealth.ExplanationText);
                    break;
                case PriusType.Kidney:
                    builder.Append(KidneyHealth.ExplanationText);
                    break;
                case PriusType.Liver:
                    builder.AppendLine(LiverHealth.ExplanationText);
                    break;
                case PriusType.Pancreas:
                    break;
            }

            return builder.ToString();
        }
    }

    public override bool Visualize(int index, HealthChoice choice) {
        bool heartChanged = HeartHealth.UpdateStatus(index, choice);
        heartIndicator.color = UpdateColor(HeartHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Heart) {
            largeHeart.DisplayOrgan(HeartHealth.score, HeartHealth.status);
        } else {
            smallHeart.DisplayOrgan(HeartHealth.score, HeartHealth.status);
        }

        bool kidneyChanged = KidneyHealth.UpdateStatus(index, choice);
        kidneyIndicator.color = UpdateColor(KidneyHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Kidney) {
            largeKidney.DisplayOrgan(KidneyHealth.score, KidneyHealth.status);
        } else {
            smallKidney.DisplayOrgan(KidneyHealth.score, KidneyHealth.status);
        }

        bool liverChanged = LiverHealth.UpdateStatus(index, choice);
        liverIndicator.color = UpdateColor(LiverHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Liver) {
            largeLiver.DisplayOrgan(LiverHealth.score, LiverHealth.status);
        } else {
            smallLiver.DisplayOrgan(LiverHealth.score, LiverHealth.status);
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
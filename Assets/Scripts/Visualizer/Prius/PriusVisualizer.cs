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

    public OrganDisplay smallHeart, largeHeart, healthyHeart,
        smallLiver, largeLiver, healthyLiver,
        smallKidney, largeKidney, healthyKidney;

    public GameObject SmallLeftKidney { get { return smallKidney.transform.GetChild(0).gameObject; } }
    public GameObject SmallRightKidney { get { return smallKidney.transform.GetChild(1).gameObject; } }
    public GameObject frame;

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

    public override bool Visualize(float index, HealthChoice choice) {
        bool heartChanged = HeartHealth.UpdateStatus(index, choice);
        heartIndicator.color = UpdateColor(HeartHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Heart) {
            largeHeart.DisplayOrgan(HeartHealth.score);
            healthyHeart.DisplayOrgan(100);
        } else {
            smallHeart.DisplayOrgan(HeartHealth.score);
        }

        bool kidneyChanged = KidneyHealth.UpdateStatus(index, choice);
        kidneyIndicator.color = UpdateColor(KidneyHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Kidney) {
            largeKidney.DisplayOrgan(KidneyHealth.score);
            healthyKidney.DisplayOrgan(100);
        } else {
            smallKidney.DisplayOrgan(KidneyHealth.score);
        }

        bool liverChanged = LiverHealth.UpdateStatus(index, choice);
        liverIndicator.color = UpdateColor(LiverHealth.status);
        if (PriusManager.Instance.currentPart == PriusType.Liver) {
            largeLiver.DisplayOrgan(LiverHealth.score);
            healthyLiver.DisplayOrgan(100);
        } else {
            smallLiver.DisplayOrgan(LiverHealth.score);
        }

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
    public void MoveOrgan(bool stl, PriusType type) {
        if (stl) {
            StartCoroutine(MoveSmallToLarge(type));
        } else {
            StartCoroutine(MoveLargeToSmall(type));
        }
    }

    /// <summary>
    /// Gets the small, large, and healthy organs depending on the type given.
    /// </summary>
    /// <returns>The organs as an array, in the order of small, large, and healthy.</returns>
    /// <param name="type">Organ type.</param>
    private OrganDisplay[] GetOrgans(PriusType type) {
        switch (type) {
            case PriusType.Heart:
                return new OrganDisplay[] { smallHeart, largeHeart, healthyHeart };
            case PriusType.Liver:
                return new OrganDisplay[] { smallLiver, largeLiver, healthyLiver };
            case PriusType.Kidney:
                return new OrganDisplay[] { smallKidney, largeKidney, healthyKidney };
            default:
                throw new System.ArgumentException("type must me one of Heart, Liver or Kidney");
        }
    }

    /// <summary>
    /// Gets the score for the corresponding organ.
    /// </summary>
    /// <returns>The score for the organ.</returns>
    /// <param name="type">Organ Type.</param>
    private int GetScore(PriusType type) {
        switch (type) {
            case PriusType.Heart:
                return HeartHealth.score;
            case PriusType.Liver:
                return LiverHealth.score;
            case PriusType.Kidney:
                return KidneyHealth.score;
            default:
                throw new System.ArgumentException("type must be one of Heart, Liver or Kidney");
        }
    }

    /// <summary>
    /// Moves the small organ to the large one.
    /// Note that for kidneys, need to confirm left or right.
    /// </summary>
    /// <param name="type">Organ type.</param>
    private IEnumerator MoveSmallToLarge(PriusType type) {
        OrganDisplay[] organs = GetOrgans(type);

        // Shows all small organs and hide all large organs.
        // The reason we do this is because the user might select a small organ
        // a large one is already in display.
        // In this situation, we should hide the already displayed large organ.
        smallHeart.gameObject.SetActive(true);
        smallLiver.gameObject.SetActive(true);
        smallKidney.gameObject.SetActive(true);
        frame.SetActive(false);
        largeHeart.gameObject.SetActive(false);
        healthyHeart.gameObject.SetActive(false);
        largeLiver.gameObject.SetActive(false);
        healthyLiver.gameObject.SetActive(false);
        largeKidney.gameObject.SetActive(false);
        healthyKidney.gameObject.SetActive(false);

        GameObject small = (type == PriusType.Kidney) ? (PriusManager.Instance.kidneyLeft ? SmallLeftKidney : SmallRightKidney): organs[0].gameObject;
        GameObject large = organs[1].gameObject;

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
        frame.SetActive(true);
        organs[2].gameObject.SetActive(true);
        organs[1].DisplayOrgan(GetScore(type));
        organs[2].DisplayOrgan(100);
    }

    /// <summary>
    /// Moves the large organ to the small one.
    /// Note that for kidneys, need to confirm left or right.
    /// </summary>
    /// <param name="type">Organ type.</param>
    private IEnumerator MoveLargeToSmall(PriusType type) {
        OrganDisplay[] organs = GetOrgans(type);

        GameObject small = (type == PriusType.Kidney) ? (PriusManager.Instance.kidneyLeft ? SmallLeftKidney : SmallRightKidney) : organs[0].gameObject;
        GameObject large = organs[1].gameObject;

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
        frame.SetActive(false);
        organs[2].gameObject.SetActive(false);
        organs[0].DisplayOrgan(GetScore(type));
    }

    public void DisplayOrgan(PriusType type) {
        switch (type) {
            case PriusType.Liver:
                largeLiver.DisplayOrgan(LiverHealth.score);
                healthyLiver.DisplayOrgan(100);
                break;
            case PriusType.Kidney:
                largeKidney.DisplayOrgan(KidneyHealth.score);
                healthyKidney.DisplayOrgan(100);
                break;
            case PriusType.Heart:
                largeHeart.DisplayOrgan(HeartHealth.score);
                healthyHeart.DisplayOrgan(100);
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
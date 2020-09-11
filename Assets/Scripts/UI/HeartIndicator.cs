using UnityEngine;

/// <summary>
/// A "Legend-of-Zelda" style hearts display to display character health.
/// There are three hearts: one light heart means bad health, two moderate, three good.
/// When all three hearts are displayed, a halo will appear to highlight good health.
/// </summary>
public class HeartIndicator : MonoBehaviour {
    [SerializeField] private GameObject badHeart, intermediateHeart, goodHeart;
    [SerializeField] private GameObject halo;

    public void Initialize() {
        badHeart.SetActive(false);
        intermediateHeart.SetActive(false);
        goodHeart.SetActive(false);
        halo.SetActive(false);
    }

    /// <summary>
    /// Control active status of each image as well as the halo.
    /// badHeart is always active.
    /// </summary>
    /// <param name="status">Status.</param>
    public void Display(HealthStatus status) {
        switch (status) {
            case HealthStatus.Good:
                goodHeart.SetActive(true);
                intermediateHeart.SetActive(true);
                badHeart.SetActive(true);
                halo.SetActive(true);
                break;
            case HealthStatus.Moderate:
                goodHeart.SetActive(false);
                intermediateHeart.SetActive(true);
                badHeart.SetActive(true);
                halo.SetActive(false);
                break;
            case HealthStatus.Bad:
                goodHeart.SetActive(false);
                intermediateHeart.SetActive(false);
                badHeart.SetActive(true);
                halo.SetActive(false);
                break;
        }
    }
}

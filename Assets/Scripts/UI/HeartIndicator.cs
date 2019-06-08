using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A "Legend-of-Zelda" style hearts display for indicating character health.
/// There are three hearts: one means bad health, two moderate, three good.
/// When the heart is good, a halo will be shown.
/// </summary>
public class HeartIndicator : MonoBehaviour {
    public GameObject badHeart, intermediateHeart, goodHeart;
    public GameObject halo;

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
                halo.SetActive(true);
                break;
            case HealthStatus.Moderate:
                goodHeart.SetActive(false);
                intermediateHeart.SetActive(true);
                halo.SetActive(false);
                break;
            case HealthStatus.Bad:
                goodHeart.SetActive(false);
                intermediateHeart.SetActive(false);
                halo.SetActive(false);
                break;
        }
    }
}

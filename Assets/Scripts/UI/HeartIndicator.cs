using UnityEngine;

/// <summary>
/// A "Legend-of-Zelda" style hearts display to display character health.
/// There are three hearts: one light heart means bad health, two moderate, three good.
/// When all three hearts are displayed, a halo will appear to highlight good health.
/// </summary>
public class HeartIndicator : MonoBehaviour {
    [SerializeField] private GameObject badHeart, intermediateHeart, goodHeart;
    private Material badMat, intermediateMat, goodMat;
    private Color color;
    
    public void Initialize() {
        badHeart.SetActive(false);
        intermediateHeart.SetActive(false);
        goodHeart.SetActive(false);
        
        badMat = badHeart.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        intermediateMat = intermediateHeart.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        goodMat = goodHeart.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        color = badMat.color;
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
                break;
            case HealthStatus.Moderate:
                goodHeart.SetActive(false);
                intermediateHeart.SetActive(true);
                badHeart.SetActive(true);
                break;
            case HealthStatus.Bad:
                goodHeart.SetActive(false);
                intermediateHeart.SetActive(false);
                badHeart.SetActive(true);
                break;
        }
    }

    public void Opaque(bool on) {
        color.a = on ? 0.6f : 1f;
        badMat.color = color;
        intermediateMat.color = color;
        goodMat.color = color;
    }
}

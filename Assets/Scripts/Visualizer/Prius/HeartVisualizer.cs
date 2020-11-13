using UnityEngine;
using UnityEngine.UI;

public class HeartVisualizer : Visualizer {
    [SerializeField] private GameObject heart;
    [SerializeField] private Image indicator;
    [SerializeField] private SlideBarPointer status;

    private Animator HeartAnimator => heart.transform.GetChild(0).GetComponent<Animator>();

    private void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            heart.SetActive(true);
            HeartAnimator.speed = 1.0f - score / 100.0f;
        }
    }

    public override bool Visualize(float index, HealthChoice choice) {
        bool heartChanged = HeartHealth.UpdateStatus(index, choice);
        indicator.color = PriusManager.Instance.colorLibrary.StatusColorDict[HeartHealth.status];
        DisplayOrgan(HeartHealth.score);
        status.SetProgress(HeartHealth.score);
        return heartChanged;
    }
}
using UnityEngine;
using UnityEngine.UI;


public class KidneyVisualizer : Visualizer {
    [SerializeField] private GameObject leftKidney, rightKidney;
    [SerializeField] private Image indicator;
    [SerializeField] private SlideBarPointer status;
    private SkinnedMeshRenderer LeftRenderer => leftKidney.GetComponent<SkinnedMeshRenderer>();
    private SkinnedMeshRenderer RightRenderer => rightKidney.GetComponent<SkinnedMeshRenderer>();

    private void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            // Both kidneys will be shown.
            leftKidney.SetActive(true);
            rightKidney.SetActive(true);
            LeftRenderer.SetBlendShapeWeight(0, 100 - score);
            RightRenderer.SetBlendShapeWeight(0, 100 - score);
        }
    }

    public override bool Visualize(float index, HealthChoice choice) {
        bool kidneyChanged = KidneyHealth.UpdateStatus(index, choice);
        indicator.color = PriusManager.Instance.colorLibrary.StatusColorDict[KidneyHealth.status];
        DisplayOrgan(KidneyHealth.score);
        status.SetProgress(KidneyHealth.score);
        return kidneyChanged;
    }
}
using UnityEngine;
using UnityEngine.UI;


public class LiverVisualizer : Visualizer {
    [SerializeField] private GameObject liver;
    [SerializeField] private Image indicator;
    [SerializeField] private SlideBarPointer status;
    
    private SkinnedMeshRenderer LiverRenderer => liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

    private void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            LiverRenderer.SetBlendShapeWeight(0, 100 - score);
        }
    }

    public override bool Visualize(float index, HealthChoice choice) {
        bool liverChanged = LiverHealth.UpdateStatus(index, choice);
        indicator.color = PriusManager.Instance.colorLibrary.StatusColorDict[LiverHealth.status];
        DisplayOrgan(LiverHealth.score);
        status.SetProgress(LiverHealth.score);
        return liverChanged;
    }
}
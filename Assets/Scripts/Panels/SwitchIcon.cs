using UnityEngine;
using UnityEngine.UI;

public class SwitchIcon : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activity, prius, stats;
    [SerializeField] private ArchetypePerformer performer;

    private void Start() {
        icon.sprite = prius; // First visualization is Activity
    }

    public void Switch() {
        StartCoroutine(performer.NextVisualization());
    }

    public void UpdateIcon() {
        switch (performer.CurrentVisualization) {
            case Visualization.Activity:
                icon.sprite = prius;
                break;
            case Visualization.Prius:
                icon.sprite = stats;
                break;
            case Visualization.Stats:
                icon.sprite = activity;
                break;
        }
    }

    public void SetActive(bool on) {
        icon.gameObject.SetActive(on);
    }
}
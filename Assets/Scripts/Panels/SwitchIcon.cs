using UnityEngine;
using UnityEngine.UI;

public class SwitchIcon : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activity, prius, stats;
    [SerializeField] private ArchetypePerformer performer;

    private void Start() {
        icon.sprite = prius; // Start with Activity
    }

    public void Switch() {
        StartCoroutine(performer.NextVisualization());
    }

    public void UpdateIcon() {
        switch (performer.CurrentVisualization) {
            case Visualization.Activity:
                // Next is Prius
                icon.sprite = prius;
                break;
            case Visualization.Prius:
                // Next is stats
                icon.sprite = stats;
                break;
            case Visualization.Stats:
                // Next is Activity
                icon.sprite = activity;
                break;
        }
    }

    public void SetActive(bool on) {
        icon.gameObject.SetActive(on);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class SwitchIcon : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activity, prius, stats;

    private ArchetypePerformer performer;

    public void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
        icon.sprite = prius; // Start with Activity
    }

    public void Switch() {
        performer.NextVisualization();
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
}
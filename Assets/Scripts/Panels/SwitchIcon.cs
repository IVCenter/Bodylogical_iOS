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
        switch (performer.CurrentVisualization) {
            case Visualization.Activity:
                // Switch to Prius
                performer.Activity.Toggle(false);
                performer.Prius.Toggle(true);
                // Next is Stats
                icon.sprite = stats;
                break;
            case Visualization.Prius:
                // Switch to Stats
                performer.Prius.Toggle(false);
                // Next is Activity
                icon.sprite = activity;
                break;
            case Visualization.LineChart:
                // Switch to Activity
                performer.Activity.Toggle(true);
                // Next is Prius
                icon.sprite = prius;
                break;
        }
    }
}
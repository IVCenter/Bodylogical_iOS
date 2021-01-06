using System.Collections;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;

    [System.Serializable]
    private class VisualizerInfo {
        public JoggingVisualizer visualizer;
        public Transform performerTransform;
    }

    [SerializeField] private VisualizerInfo[] visualizers;
    
    private bool initialized;

    // Wheelchair prefabs
    public GameObject wheelchairPrefab;

    // Tutorials
    [SerializeField] private Transform activityTutorialTransform;
    public bool TutorialShown { get; set; }

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Toggles the performers, creating new if required.
    /// </summary>
    public void ToggleActivity(bool on) {
        if (on) {
            if (!initialized) {
                initialized = true;
                
                // The "true" avatar will stand in middle
                foreach (VisualizerInfo info in visualizers) {
                    info.visualizer.PerformerTransform = info.performerTransform;
                }

                // TODO
                //visualizers[1].visualizer.Performer = ArchetypeManager.Instance.Selected;
                // visualizers[0].visualizer.Performer = new ArchetypeModel(
                //     ArchetypeManager.Instance.Selected.ArchetypeData, visualizers[0].performerTransform);
                // visualizers[2].visualizer.Performer = new ArchetypeModel(
                //     ArchetypeManager.Instance.Selected.ArchetypeData, visualizers[2].performerTransform);
            }
            
            foreach (VisualizerInfo info in visualizers) {
                info.visualizer.Performer.Heart.gameObject.SetActive(true);
                info.visualizer.Performer.Heart.Initialize();
            }
        } else if (initialized) {
            foreach (VisualizerInfo info in visualizers) {
                info.visualizer.Performer.Heart.gameObject.SetActive(false);
                info.visualizer.Stop();
            }
        }
    }

    /// <summary>
    /// Switch to Activity view.
    /// </summary>
    public void StartActivity() {
        if (!TutorialShown) {
            TutorialParam text = new TutorialParam("Tutorials.ActivityTitle", "Tutorials.ActivityText");
            TutorialManager.Instance.ShowTutorial(text, activityTutorialTransform,
                () => TimeProgressManager.Instance.Playing, postCallback: TimeProgressManager.Instance.ShowTut1);
            TutorialShown = true;
        }

        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(float index, HealthChoice choice) {
        foreach (VisualizerInfo info in visualizers) {
            info.visualizer.Visualize(index, choice);
        }
    }

    public void Reset() {
        for (int i = 0; i < visualizers.Length; i++) {
            visualizers[i].visualizer.ResetVisualizer();
            if (i != 1) {
                visualizers[i].visualizer.Performer.Dispose();
            }
        }
        
        activityParent.SetActive(false);
        initialized = false;
    }
}
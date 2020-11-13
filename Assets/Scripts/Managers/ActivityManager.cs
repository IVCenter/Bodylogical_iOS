using System.Collections;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;
    [SerializeField] private TreadmillVisualizer[] visualizers;
    
    // Archetype and two replicas
    public Transform[] performerPositions; // For the two replicas only
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
                for (int i = 0; i < visualizers.Length; i++) {
                    visualizers[i].PerformerTransform = performerPositions[i];
                }
                
                visualizers[1].Performer = ArchetypeManager.Instance.Selected;
                visualizers[0].Performer = new ArchetypeModel(ArchetypeManager.Instance.Selected.ArchetypeData, performerPositions[0]);
                visualizers[2].Performer = new ArchetypeModel(ArchetypeManager.Instance.Selected.ArchetypeData, performerPositions[2]);
            }

            visualizers[0].Performer.InfoCanvas.SetActive(false);
            visualizers[2].Performer.InfoCanvas.SetActive(false);
            foreach (TreadmillVisualizer visualizer in visualizers) {
                visualizer.Performer.Heart.gameObject.SetActive(true);
                visualizer.Performer.Heart.Initialize();
            }
        } else if (initialized) {
            foreach (TreadmillVisualizer visualizer in visualizers) {
                visualizer.Performer.Heart.gameObject.SetActive(false);
                visualizer.Stop();
            }
        }
    }

    /// <summary>
    /// Switch to Activity view.
    /// </summary>
    public IEnumerator StartActivity(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, activityParent);

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
        foreach (TreadmillVisualizer visualizer in visualizers) {
            visualizer.Visualize(index, choice);
        }
    }

    public void Reset() {
        for (int i = 0; i < visualizers.Length; i++) {
            visualizers[i].ResetVisualizer();
            if (i != 1) {
                visualizers[i].Performer.Dispose();
            }
        }
        
        activityParent.SetActive(false);
        initialized = false;
    }
}
using System.Collections;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;

    // All activities
    [SerializeField] private GameObject[] activities;
    private Visualizer[] visualizers;
    private int currentIndex; // current visualization

    // Archetype and two replicas
    public ArchetypeModel[] Performers { get; private set; }
    public Transform[] performerPositions; // For the two replicas only
    private bool initialized;

    // Wheelchair prefabs
    public GameObject maleCompanionPrefab;
    public GameObject femaleCompanionPrefab;
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

        visualizers = new Visualizer[activities.Length];
        for (int i = 0; i < activities.Length; i++) {
            visualizers[i] = activities[i].GetComponent<Visualizer>();
        }
    }

    /// <summary>
    /// Toggles the performers, creating new if required.
    /// </summary>
    public void ToggleActivity(bool on) {
        if (on) {
            if (!initialized) {
                initialized = true;

                Performers = new ArchetypeModel[3];
                // The "true" avatar will stand in middle
                Performers[1] = ArchetypeManager.Instance.Selected;
                Performers[0] = new ArchetypeModel(Performers[1].ArchetypeData, performerPositions[0]);
                Performers[2] = new ArchetypeModel(Performers[1].ArchetypeData, performerPositions[2]);
            }

            Performers[0].InfoCanvas.SetActive(false);
            Performers[2].InfoCanvas.SetActive(false);
            Performers[0].Heart.gameObject.SetActive(true);
            Performers[1].Heart.gameObject.SetActive(true);
            Performers[2].Heart.gameObject.SetActive(true);
            Performers[0].Heart.Initialize();
            Performers[1].Heart.Initialize();
            Performers[2].Heart.Initialize();
        } else if (initialized) {
            Performers[0].Heart.gameObject.SetActive(false);
            Performers[1].Heart.gameObject.SetActive(false);
            Performers[2].Heart.gameObject.SetActive(false);
            visualizers[currentIndex].Stop();
        }
    }

    /// <summary>
    /// Switch to Activity view.
    /// </summary>
    public IEnumerator StartActivity(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, activityParent, true);

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
        visualizers[currentIndex].Visualize(index, choice);
    }

    /// <summary>
    /// Switches between three activities.
    /// </summary>
    /// <param name="index">Index.</param>
    public void SwitchActivity(int index) {
        visualizers[currentIndex].Stop();
        activities[currentIndex].SetActive(false);
        currentIndex = index;
        activities[currentIndex].SetActive(true);
        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
    }

    public void Reset() {
        visualizers[currentIndex].ResetVisualizer();
        if (Performers != null) {
            Performers[0].Dispose();
            Performers[2].Dispose();
        }
        activityParent.SetActive(false);
        initialized = false;
    }
}
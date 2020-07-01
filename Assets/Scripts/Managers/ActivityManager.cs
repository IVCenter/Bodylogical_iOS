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
    [HideInInspector] public ArchetypeModel[] performers;
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

                performers = new ArchetypeModel[3];
                // The "true" avatar will stand in middle
                performers[1] = ArchetypeManager.Instance.Selected;
                performers[0] = new ArchetypeModel(performers[1].ArchetypeData, performerPositions[0]);
                performers[2] = new ArchetypeModel(performers[1].ArchetypeData, performerPositions[2]);
            }

            performers[0].InfoCanvas.SetActive(false);
            performers[2].InfoCanvas.SetActive(false);
            performers[0].Heart.gameObject.SetActive(true);
            performers[1].Heart.gameObject.SetActive(true);
            performers[2].Heart.gameObject.SetActive(true);
            performers[0].Heart.Initialize();
            performers[1].Heart.Initialize();
            performers[2].Heart.Initialize();
        } else if (initialized) {
            performers[0].Heart.gameObject.SetActive(false);
            performers[1].Heart.gameObject.SetActive(false);
            performers[2].Heart.gameObject.SetActive(false);
        }
        visualizers[currentIndex].Stop();
    }

    /// <summary>
    /// Switch to Activity view.
    /// </summary>
    public IEnumerator StartActivity(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, activityParent, true);

        if (!TutorialShown) {
            TutorialManager.Instance.ClearTutorial();
            // TODO: new tutorial about the control panel
            //TutorialParam text = new TutorialParam("Tutorials.ActIntroTitle", "Tutorials.ActIntroText");
            //TutorialManager.Instance.ShowTutorial(text, activityTutorialTransform);
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
        performers[0].Dispose();
        performers[2].Dispose();
        activityParent.SetActive(false);
        initialized = false;
    }
}
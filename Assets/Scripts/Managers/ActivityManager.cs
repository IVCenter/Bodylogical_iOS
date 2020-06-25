using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;

    // All activities
    [SerializeField] private List<GameObject> activities;
    private List<Visualizer> visualizers;
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
    [HideInInspector] public bool tutorialShown;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        visualizers = new List<Visualizer>();
        foreach (GameObject activity in activities) {
            visualizers.Add(activity.GetComponent<Visualizer>());
        }
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// Notice: does NOT toggle parent object (left to StartActivity).
    /// </summary>
    public void ToggleActivity(bool on) {
        if (!initialized && on) {
            initialized = true;

            performers = new ArchetypeModel[3];
            performers[1] = ArchetypeManager.Instance.Selected;
            performers[0] = new ArchetypeModel(performers[1].archetype);
            performers[0].model.transform.SetParent(performerPositions[0], false);
            performers[2] = new ArchetypeModel(performers[1].archetype);
            performers[2].model.transform.SetParent(performerPositions[2], false);

            performers[0].infoCanvas.SetActive(false);
            performers[2].infoCanvas.SetActive(false);
            performers[0].heart.gameObject.SetActive(true);
            performers[1].heart.gameObject.SetActive(true);
            performers[2].heart.gameObject.SetActive(true);
            performers[0].heart.Initialize();
            performers[1].heart.Initialize();
            performers[2].heart.Initialize();
        } else if (initialized && !on) {
            performers[0].heart.gameObject.SetActive(false);
            performers[1].heart.gameObject.SetActive(false);
            performers[2].heart.gameObject.SetActive(false);
        }
        visualizers[currentIndex].Pause();
    }

    /// <summary>
    /// Switch to Animations view.
    /// </summary>
    public IEnumerator StartActivity(GameObject orig) {
        yield return StageManager.Instance.ChangeVisualization(orig, activityParent, true);

        performers[0].heart.Display(HealthStatus.Bad);
        performers[1].heart.Display(HealthStatus.Moderate);
        performers[2].heart.Display(HealthStatus.Good);

        if (!tutorialShown) {
            TutorialManager.Instance.ClearTutorial();
            // TODO: new tutorial about the control panel
            //TutorialParam text = new TutorialParam("Tutorials.ActIntroTitle", "Tutorials.ActIntroText");
            //TutorialManager.Instance.ShowTutorial(text, activityTutorialTransform);
            tutorialShown = true;
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
        visualizers[currentIndex].Pause();
        activities[currentIndex].SetActive(false);
        currentIndex = index;
        activities[currentIndex].SetActive(true);
        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
    }

    public void Reset() {
        visualizers[currentIndex].Pause();
        visualizers[currentIndex].Reset();
        performers[0].Dispose();
        performers[2].Dispose();
        activityParent.SetActive(false);
        initialized = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager to control the animations, formerly known as the "props".
/// </summary>
public class ActivityManager : MonoBehaviour {
    public static ActivityManager Instance { get; private set; }

    public GameObject activityParent;
    // Only one activity is available now.
    [SerializeField] private List<GameObject> activities;
    [SerializeField] private CompanionController maleController;
    [SerializeField] private CompanionController femaleController;

    public CompanionController CurrentCompanion =>
        ArchetypeManager.Instance.selectedArchetype.gender == Gender.Male ?
                maleController : femaleController;

    public CompanionController OtherCompanion =>
        ArchetypeManager.Instance.selectedArchetype.gender == Gender.Male ?
                femaleController : maleController;

    public Transform CurrentTransform => CurrentCompanion.transform;
    public Transform OtherTransform => OtherCompanion.transform;
    public Animator CurrentAnimator => CurrentCompanion.companionAnimator;
    public Animator OtherAnimator => OtherCompanion.companionAnimator;

    public WheelchairController wheelchair;

    [SerializeField]
    private Vector3 companionOriginalLocalPos;

    public HeartIndicator charHeart, compHeart;

    private List<Visualizer> visualizers;
    private int currentIndex;

    [SerializeField]
    private Transform activityTutorialTransform;
    [HideInInspector]
    public bool tutorialShown;

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
    /// Switch to Animations view.
    /// </summary>
    public IEnumerator StartActivity(GameObject orig) {
        // after stage is shown
        yield return ArchetypeManager.Instance.MoveSelectedArchetypeToLeft();
        OtherCompanion.gameObject.SetActive(false);
        CurrentCompanion.gameObject.SetActive(true);
        CurrentTransform.localPosition = companionOriginalLocalPos;
        yield return StageManager.Instance.ChangeVisualization(orig, activityParent);

        if (!tutorialShown) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam text = new TutorialParam("Tutorials.ActIntroTitle", "Tutorials.ActIntroText");
            TutorialManager.Instance.ShowTutorial(text, activityTutorialTransform);
            tutorialShown = true;
        }

        Visualize(TimeProgressManager.Instance.YearValue / 5, TimeProgressManager.Instance.Path);
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// Notice: does NOT toggle parent object (left to StartActivity).
    /// </summary>
    public void ToggleActivity(bool on) {
        //ControlPanelManager.Instance.ToggleActivitySelector(on);
        //ControlPanelManager.Instance.ToggleTimeControls(on);

        visualizers[currentIndex].Pause();
    }

    /// <summary>
    /// Play the animation.
    /// </summary>
    public void Visualize(float index, HealthChoice choice) {
        CurrentCompanion.ToggleLegend(true);
        compHeart.Display(HealthStatus.Good);
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
        activityParent.SetActive(false);
    }
}
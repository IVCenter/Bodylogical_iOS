using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager that controls the archetypes.
/// </summary>
public class ArchetypeManager : MonoBehaviour {
    public static ArchetypeManager Instance { get; private set; }

    /// <summary>
    /// Position for tutorial.
    /// </summary>
    [SerializeField] private Transform tutorialTransform;
    
    public ArchetypeDisplayer displayer;
    public ArchetypePerformer[] performers;

    public GameObject PerformerParent => performers[0].transform.parent.gameObject;

    // Animator property hashes
    private static readonly int Greetings = Animator.StringToHash("Greetings");

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    #region State: PlaceStage

    /// <summary>
    /// Called when stage is settled. Loop among different poses.
    /// </summary>
    public void SetGreetingPoses(bool on) {
        displayer.Anim.SetBool(Greetings, on);
    }

    #endregion

    #region State: PickArchetype

    /// <summary>
    /// Starts a coroutine to move the displayer, as well as the detail panels, to the specified position.
    /// </summary>
    public IEnumerator MoveDisplayerTo(Vector3 endPos) {
        displayer.panel.GetComponent<LockRotation>().StartLock();
        displayer.Header.GetComponent<LockRotation>().StartLock();
        yield return displayer.MoveTo(endPos);
        displayer.panel.GetComponent<LockRotation>().EndLock();
        displayer.Header.GetComponent<LockRotation>().EndLock();
    }

    #endregion

    /// <summary>
    /// De-select the avatar and let the user to select a new avatar.
    /// </summary>
    public void ResetAvatars() {
        // Put the selected archetype back
        displayer.model.transform.localPosition = Vector3.zero;
        displayer.Reset();
        SetGreetingPoses(true);
        // Destroy all performers
        // foreach (ArchetypePerformer performer in performers) {
        //     performer.Dispose();
        // }
        //
        // Performers.Clear();
        PerformerParent.SetActive(false);
    }

    #region Tutorials

    public void LifestyleTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.LifestyleTitle", "Tutorials.LifestyleText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform,
            () => displayer.panel.AllClicked,
            postCallback: VisualizationTutorial);
    }

    private void VisualizationTutorial() {
        TutorialParam param = new TutorialParam("Tutorials.VisualizationTitle", "Tutorials.VisualizationText");
        TutorialManager.Instance.ShowTutorial(param, tutorialTransform,
            () => AppStateManager.Instance.CurrState == AppState.Visualizations,
            postCallback: StageManager.Instance.ActivityTutorial
        );
    }

    #endregion
}
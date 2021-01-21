using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager that controls the archetypes.
/// </summary>
public class ArchetypeManager : MonoBehaviour {
    public static ArchetypeManager Instance { get; private set; }

    /// <summary>
    /// Positions for all the base archetypes.
    /// </summary>
    [SerializeField] private Transform[] displayerTransforms;

    /// <summary>
    /// Positions for the three archetype models.
    /// </summary>
    [SerializeField] private Transform[] performerTransforms;

    public GameObject displayerPrefab;
    public GameObject performerPrefab;
    
    private List<ArchetypeDisplayer> displayers;

    public Transform PerformerParent => performerTransforms[0].parent;
    public bool StartSelectArchetype { get; set; }
    public ArchetypeDisplayer Selected { get; set; }
    public Dictionary<HealthChoice, ArchetypePerformer> Performers { get; private set; }

    private bool modelsLoaded;

    // Animator property hashes
    private static readonly int greetings = Animator.StringToHash("Greetings");

    #region Unity routines

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        displayers = new List<ArchetypeDisplayer>();
        Performers = new Dictionary<HealthChoice, ArchetypePerformer>();
    }

    #endregion

    #region State: PlaceStage

    /// <summary>
    /// For each of the archetypes, create a model.
    /// </summary>
    public void LoadArchetypes() {
        if (!modelsLoaded) {
            // Load the archetypes
            List<Archetype> archetypes = DataLoader.LoadArchetypes();
            // Number of archetypes to be loaded
            int numArchetypes = Mathf.Min(archetypes.Count, displayerTransforms.Length);
            for (int i = 0; i < numArchetypes; i++) {
                Archetype archetype = archetypes[i];
                ArchetypeDisplayer archetypeDisplayer = new ArchetypeDisplayer(archetype, displayerTransforms[i]);
                archetypeDisplayer.Model.GetComponentInChildren<ArchetypeInteract>().Displayer = archetypeDisplayer;
                displayers.Add(archetypeDisplayer);
            }

            modelsLoaded = true;
        }
    }

    /// <summary>
    /// Called when stage is settled. Loop among different poses.
    /// </summary>
    public void SetGreetingPoses(bool on) {
        foreach (ArchetypeDisplayer archetypeBase in displayers) {
            archetypeBase.ArchetypeAnimator.SetBool(greetings, on);
        }
    }

    #endregion

    #region State: PickArchetype

    /// <summary>
    /// Starts a coroutine to move the selected display, as well as the detail panels, to the specified position.
    /// </summary>
    public IEnumerator MoveSelectedTo(Vector3 endPos) {
        Selected.Panel.GetComponent<LockRotation>().StartLock();
        Selected.Header.GetComponent<LockRotation>().StartLock();
        yield return Selected.MoveTo(endPos);
        Selected.Panel.GetComponent<LockRotation>().EndLock();
        Selected.Header.GetComponent<LockRotation>().EndLock();
    }
    
    /// <summary>
    /// Toggles all unselected archetypes.
    /// </summary>
    public void ToggleUnselectedArchetype(bool on) {
        foreach (ArchetypeDisplayer archetypeBase in displayers) {
            if (archetypeBase.Model != Selected.Model) {
                archetypeBase.Model.SetActive(on);
            }
        }
    }

    /// <summary>
    /// Creates three ArchetypeModels for the base, one for each intervention/HealthChoice.
    /// </summary>
    public void CreateModels() {
        Dictionary<HealthChoice, Lifestyle> lifestyles = DataLoader.LoadLifestyles(Selected.ArchetypeData);
        Dictionary<HealthChoice, LongTermHealth> healthData = DataLoader.LoadHealthData(Selected.ArchetypeData);

        Performers[HealthChoice.None] = new ArchetypePerformer(Selected.ArchetypeData, performerTransforms[0],
            HealthChoice.None, lifestyles[HealthChoice.None], healthData[HealthChoice.None],
            StageManager.Instance.props[0]);
        Performers[HealthChoice.Minimal] = new ArchetypePerformer(Selected.ArchetypeData, performerTransforms[1],
            HealthChoice.Minimal, lifestyles[HealthChoice.Minimal], healthData[HealthChoice.Minimal],
            StageManager.Instance.props[1]);
        Performers[HealthChoice.Optimal] = new ArchetypePerformer(Selected.ArchetypeData, performerTransforms[2],
            HealthChoice.Optimal, lifestyles[HealthChoice.Optimal], healthData[HealthChoice.Optimal],
        StageManager.Instance.props[2]);
    }

    #endregion

    /// <summary>
    /// De-select the avatar and let the user to select a new avatar.
    /// </summary>
    public void Reset() {
        // In Prius the avatar might not be visible.
        // Enable selected avatar and hide all organs.
        Selected.Model.SetActive(true);
        // Put the selected archetype back
        Selected.Model.transform.localPosition = Vector3.zero;
        Selected.Model.transform.Search("BasicInfoCanvas").gameObject.SetActive(true);
        ToggleUnselectedArchetype(true);
        Selected = null;
        SetGreetingPoses(true);
    }
}
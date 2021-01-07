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
    
    /// <summary>
    /// The four detail panels.
    /// </summary>
    public DetailPanel detailPanel;

    public GameObject displayerPrefab;
    public GameObject performerPrefab;

    private const float epsilon = 0.001f;
    private List<ArchetypeDisplayer> displayers;

    public Transform PerformerParent => performerTransforms[0].parent;
    public bool StartSelectArchetype { get; set; }
    public ArchetypeDisplayer Selected { get; set; }
    public Dictionary<HealthChoice, ArchetypePerformer> Performers { get; private set; }

    private bool modelsLoaded;

    // Animator property hashes
    private static readonly int greetings = Animator.StringToHash("Greetings");
    private static readonly int walk = Animator.StringToHash("Walk");

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
        if (Selected != null) {
            Transform trans = Selected.Model.transform;
            Vector3 forward = transform.forward;

            // Calculate if the archetype needs to travel, and if so, which direction to rotate
            Vector3 startPos = trans.position;
            Vector3 direction = startPos - endPos;
            direction.y = 0; // Ignore elevation
            direction = Vector3.Normalize(direction);

            if (Vector3.Distance(startPos, endPos) < epsilon) {
                // epsilon value, no need to move
                yield break;
            }

            Vector3 rotation = trans.localEulerAngles;
            float startAngle = rotation.y;
            float targetAngle = Vector3.SignedAngle(forward, direction, Vector3.up);
            float progress;

            // Rotate archetype
            for (progress = 0; progress < 1; progress += 0.02f) {
                rotation.y = startAngle + Mathf.SmoothStep(0, targetAngle, progress);
                trans.localEulerAngles = rotation;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // Move archetype
            Transform panelTransform = detailPanel.transform;
            Selected.ArchetypeAnimator.SetBool(walk, true);
            for (progress = 0; progress < 1; progress += 0.01f) {
                Vector3 newPos = new Vector3(
                    Mathf.SmoothStep(startPos.x, endPos.x, progress),
                    Mathf.SmoothStep(startPos.y, endPos.y, progress),
                    Mathf.SmoothStep(startPos.z, endPos.z, progress)
                );
                trans.position = newPos;
                panelTransform.position = newPos;
                yield return null;
            }

            trans.position = endPos;
            Selected.ArchetypeAnimator.SetBool(walk, false);
            yield return new WaitForSeconds(0.5f);

            // Rotate back
            for (progress = 0; progress < 1; progress += 0.02f) {
                rotation.y = startAngle + Mathf.SmoothStep(targetAngle, 0, progress);
                trans.localEulerAngles = rotation;
                yield return null;
            }
        }

        yield return null;
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
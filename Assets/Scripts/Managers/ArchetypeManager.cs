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
    [SerializeField] private Transform[] baseTransforms;

    /// <summary>
    /// Positions for the three archetype models.
    /// </summary>
    [SerializeField] private Transform[] modelTransforms;
    
    public GameObject basePrefab;
    public GameObject modelPrefab;
    
    private const float epsilon = 0.001f;
    private List<ArchetypeBase> archetypeBases;

    public bool StartSelectArchetype { get; set; }
    public ArchetypeBase Selected { get; set; } = null;
    public Dictionary<HealthChoice, ArchetypeModel> Models { get; private set; }

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

        archetypeBases = new List<ArchetypeBase>();
        Models = new Dictionary<HealthChoice, ArchetypeModel>();
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
            int numArchetypes = Mathf.Min(archetypes.Count, baseTransforms.Length);
            for (int i = 0; i < numArchetypes; i++) {
                Archetype archetype = archetypes[i];
                ArchetypeBase archetypeBase = new ArchetypeBase(archetype, baseTransforms[i]);
                archetypeBase.Model.GetComponentInChildren<ArchetypeInteract>().Base = archetypeBase;
                archetypeBases.Add(archetypeBase);
            }

            modelsLoaded = true;
        }
    }

    /// <summary>
    /// Called when stage is settled. Loop among different poses.
    /// </summary>
    public void SetGreetingPoses(bool on) {
        foreach (ArchetypeBase archetypeBase in archetypeBases) {
            archetypeBase.ArchetypeAnimator.SetBool(greetings, on);
        }
    }

    #endregion

    #region State: PickArchetype

    /// <summary>
    /// Starts a coroutine to move the selected model to center of stage.
    /// </summary>
    public IEnumerator MoveSelectedToCenter() {
        if (Selected != null) {
            Transform trans = Selected.Model.transform;

            // Calculate if the archetype needs to travel, and if so, which direction to rotate
            Vector3 startPos = trans.position;
            Vector3 endPos = StageManager.Instance.stageCenter.position;
            Vector3 direction = Vector3.Normalize(endPos - startPos);
            if (Vector3.Distance(startPos, endPos) < epsilon) {
                // epsilon value, no need to move
                yield break;
            }

            float progress;

            // Rotate archetype
            float targetAngle = direction.x < 0 ? 90 : -90;
            Vector3 rotation = trans.localEulerAngles;
            for (progress = 0; progress < 1; progress += 0.02f) {
                rotation.y = Mathf.SmoothStep(0, targetAngle, progress);
                trans.localEulerAngles = rotation;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // Move archetype
            Selected.ArchetypeAnimator.SetBool(walk, true);
            for (progress = 0; progress < 1; progress += 0.01f) {
                trans.position = new Vector3(
                    Mathf.SmoothStep(startPos.x, endPos.x, progress),
                    Mathf.SmoothStep(startPos.y, endPos.y, progress),
                    Mathf.SmoothStep(startPos.z, endPos.z, progress)
                );
                yield return null;
            }

            trans.position = endPos;
            Selected.ArchetypeAnimator.SetBool(walk, false);
            yield return new WaitForSeconds(0.5f);

            // Rotate back
            for (progress = 0; progress < 1; progress += 0.02f) {
                rotation.y = Mathf.SmoothStep(targetAngle, 0, progress);
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
        foreach (ArchetypeBase archetypeBase in archetypeBases) {
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
        
        Models[HealthChoice.None] = new ArchetypeModel(Selected.ArchetypeData, modelTransforms[0],
            lifestyles[HealthChoice.None], healthData[HealthChoice.None]);
        Models[HealthChoice.Minimal] = new ArchetypeModel(Selected.ArchetypeData, modelTransforms[1],
            lifestyles[HealthChoice.Minimal], healthData[HealthChoice.Minimal]);
        Models[HealthChoice.Optimal] = new ArchetypeModel(Selected.ArchetypeData, modelTransforms[2],
            lifestyles[HealthChoice.Optimal], healthData[HealthChoice.Optimal]);
    }

    #endregion

    #region State: ShowDetails

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

    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager that controls the archetypes.
/// </summary>
public class ArchetypeManager : MonoBehaviour {
    public static ArchetypeManager Instance { get; private set; }

    /// <summary>
    /// Positions for all the archetypes.
    /// </summary>
    [SerializeField] private Transform[] archetypeTransforms;

    private const float epsilon = 0.001f;
    private List<ArchetypeModel> archetypeModels;

    public ArchetypeModel Selected { get; private set; }
    public bool ArchetypeSelected { get; set; }
    public bool StartSelectArchetype { get; set; }

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

        archetypeModels = new List<ArchetypeModel>();
    }

    private void Update() {
        if (!ArchetypeSelected) {
            if (StartSelectArchetype && CheckSelection()) {
                ArchetypeSelected = true;
                StartSelectArchetype = false;
            }
        }
    }

    #endregion

    #region State: PlaceStage

    /// <summary>
    /// For each of the archetypes, create a model.
    /// </summary>
    public void LoadArchetypes() {
        if (!modelsLoaded) {
            // Number of archetypes to be loaded
            int numArchetypes = Mathf.Min(ArchetypeLoader.Instance.Profiles.Count, archetypeTransforms.Length);
            for (int i = 0; i < numArchetypes; i++) {
                Archetype archetype = ArchetypeLoader.Instance.Profiles[i];
                ArchetypeModel archetypeModel = new ArchetypeModel(archetype, archetypeTransforms[i]);
                archetypeModels.Add(archetypeModel);
            }

            modelsLoaded = true;
        }
    }

    /// <summary>
    /// Called when stage is settled. Loop among different poses.
    /// </summary>
    public void SetGreetingPoses(bool on) {
        foreach (ArchetypeModel archetypeModel in archetypeModels) {
            archetypeModel.ArchetypeAnimator.SetBool(greetings, on);
        }
    }

    #endregion

    #region State: PickArchetype

    /// <summary>
    /// Checks if a human model is selected.
    /// </summary>
    /// <returns>true if a model is selected, false otherwise.</returns>
    private bool CheckSelection() {
        foreach (ArchetypeModel archetypeModel in archetypeModels) {
            if (archetypeModel.Model.GetComponentInChildren<ArchetypeInteract>().IsSelected) {
                Selected = archetypeModel;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Starts a coroutine to move the selected model to center of stage.
    /// </summary>
    public IEnumerator MoveSelectedToCenter() {
        if (!ArchetypeSelected) {
            yield break;
        }

        if (ArchetypeSelected && Selected.Model != null) {
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
        foreach (ArchetypeModel archetypeModel in archetypeModels) {
            if (archetypeModel.Model != Selected.Model) {
                archetypeModel.Model.SetActive(on);
            }
        }
    }

    #endregion

    #region State: ShowDetails

    /// <summary>
    /// Expand selected profile details.
    /// </summary>
    public void ExpandArchetypeInfo() {
        if (Selected == null || Selected.Model == null) {
            return;
        }

        DetailPanelManager.Instance.ToggleDetailPanel(true);
        DetailPanelManager.Instance.SetValues();
    }

    /// <summary>
    /// Moves the avatar toward left.
    /// </summary>
    public IEnumerator MoveSelectedToLeft() {
        if (ArchetypeSelected && Selected.Model != null) {
            Transform trans = Selected.Model.transform;
            
            // Determine if we need to move the avatar
            Vector3 startPos = trans.position;
            Vector3 center = StageManager.Instance.stageCenter.position;
            Vector3 endPos = new Vector3(center.x - 0.2f, center.y, center.z);
            if (Vector3.Distance(startPos, endPos) < epsilon) {
                yield break;
            }

            float progress;
            
            // Rotate archetype
            Vector3 rotation = trans.localEulerAngles;
            for (progress = 0; progress < 1; progress += 0.02f) {
                rotation.y = Mathf.SmoothStep(0, 90, progress);
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
                progress += 0.01f;
                yield return null;
            }
            trans.position = endPos;
            Selected.ArchetypeAnimator.SetBool(walk, false);
            yield return new WaitForSeconds(0.5f);
            
            // Rotate back
            for (progress = 0; progress < 1; progress += 0.02f) {
                rotation.y = Mathf.SmoothStep(90, 0, progress);
                trans.localEulerAngles = rotation;
                yield return null;
            }
        }
    }

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
        Selected.Model.GetComponentInChildren<ArchetypeInteract>().IsSelected = false;
        ToggleUnselectedArchetype(true);
        ArchetypeSelected = false;
        Selected = null;
        SetGreetingPoses(true);
    }

    #endregion
}
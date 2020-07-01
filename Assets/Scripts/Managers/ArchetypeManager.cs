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
    private List<ArchetypeModel> archetypeModels;

    public ArchetypeModel Selected { get; private set; }
 
    [HideInInspector] public bool archetypeSelected;
    [HideInInspector] public bool startSelectArchetype;
    private bool modelsLoaded;

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
        if (!archetypeSelected) {
            if (startSelectArchetype && CheckSelection()) {
                archetypeSelected = true;
                startSelectArchetype = false;
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
            int numArchetypes = Mathf.Min(
                ArchetypeLoader.Instance.Profiles.Count,
                archetypeTransforms.Length);
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
    public void SetGreetingPoses() {
        foreach (ArchetypeModel archetypeModel in archetypeModels) {
            archetypeModel.ArchetypeAnimator.SetBool("Greetings", true);
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
        if (!archetypeSelected) {
            yield break;
        }

        if (archetypeSelected && Selected.Model != null) {
            float movedDist = 0;

            Vector3 startPos = Selected.Model.transform.position;
            Vector3 endPos = StageManager.Instance.stageCenter.position;
            float journeyLength = Vector3.Distance(startPos, endPos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                Selected.Model.transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            Selected.Model.transform.position = endPos;
            Selected.Model.transform.rotation = StageManager.Instance.stage.transform.rotation;
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

        Selected.InfoCanvas.SetActive(false);
        Selected.ArchetypeAnimator.SetBool("Greetings", false);
        DetailPanelManager.Instance.ToggleDetailPanel(true);
        DetailPanelManager.Instance.SetValues();
    }

    /// <summary>
    /// Moves the avatar toward left.
    /// </summary>
    public IEnumerator MoveSelectedToLeft() {
        if (archetypeSelected && Selected.Model != null) {
            float movedDist = 0;
            Vector3 startpos = Selected.Model.transform.position;
            Vector3 center = StageManager.Instance.stageCenter.position;
            Vector3 endpos = new Vector3(center.x - 0.2f, center.y, center.z);

            float journeyLength = Vector3.Distance(startpos, endpos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                Selected.Model.transform.position = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            Selected.Model.transform.position = endpos;
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
        archetypeSelected = false;
        Selected = null;
        SetGreetingPoses();
    }
    #endregion
}
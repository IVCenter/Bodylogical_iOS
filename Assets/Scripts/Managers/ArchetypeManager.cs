using System.Collections;
using UnityEngine;

/// <summary>
/// A manager that controls the human archetypes.
/// </summary>
public class ArchetypeManager : MonoBehaviour {
    public static ArchetypeManager Instance { get; private set; }

    [HideInInspector]
    public Archetype selectedArchetype;
    public GameObject SelectedModel => selectedArchetype.Model;
    public Animator ModelAnimator =>
        SelectedModel.transform.Find("model").GetChild(0).GetComponent<Animator>();

    [HideInInspector]
    public bool archetypeSelected;
    [HideInInspector]
    public bool startSelectArchetype;

    #region Unity routines
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!archetypeSelected) {
            if (startSelectArchetype && CheckSelection()) {
                archetypeSelected = true;
                startSelectArchetype = false;
            }
        }
    }
    #endregion

    #region State: PickArchetype
    /// <summary>
    /// Checks if a human model is selected.
    /// </summary>
    /// <returns><c>true</c>, if a model is selected, <c>false</c> otherwise.</returns>
    private bool CheckSelection() {
        foreach (Archetype archetype in ArchetypeLoader.Instance.profiles) {
            if (archetype.Model.GetComponentInChildren<ArchetypeInteract>().isSelected) {
                selectedArchetype = archetype;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Starts a coroutine to move the selected model to center of stage.
    /// </summary>
    public bool MoveSelectedArchetypeToCenter() {
        if (!archetypeSelected) {
            return false;
        }

        StartCoroutine(MoveArchetypeToCenter());
        return true;
    }

    /// <summary>
    /// Moves the human toward center.
    /// </summary>
    IEnumerator MoveArchetypeToCenter() {
        if (archetypeSelected && SelectedModel != null) {
            float movedDist = 0;

            Vector3 startPos = SelectedModel.transform.position;
            Vector3 endPos = StageManager.Instance.CenterTransform.position;
            float journeyLength = Vector3.Distance(startPos, endPos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                SelectedModel.transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedModel.transform.position = endPos;
            SelectedModel.transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// Toggles all unselected human.
    /// </summary>
    public void ToggleUnselectedArchetype(bool on) {
        foreach (Archetype human in ArchetypeLoader.Instance.profiles) {
            if (human.Model != SelectedModel) {
                human.Model.SetActive(on);
            }
        }
    }
    #endregion

    #region State: ShowDetails
    /// <summary>
    /// Expand selected profile details.
    /// </summary>
    public void ExpandArchetypeInfo() {
        if (selectedArchetype == null || SelectedModel == null) {
            return;
        }

        SelectedModel.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        DetailPanelManager.Instance.ToggleDetailPanel(true);
        DetailPanelManager.Instance.SetValues();
    }

    /// <summary>
    /// Prepares the human model for visualization.
    /// </summary>
    public void PrepareVisualization() {
        DetailPanelManager.Instance.ToggleDetailPanel(false);
        //ControlPanelManager.Instance.TogglePredictPanel(false);
        StartCoroutine(MoveSelectedArchetypeToLeft());
    }

    /// <summary>
    /// Moves the human toward left.
    /// </summary>
    public IEnumerator MoveSelectedArchetypeToLeft() {
        if (archetypeSelected && SelectedModel != null) {
            float movedDist = 0;

            Vector3 startpos = SelectedModel.transform.localPosition;
            Vector3 center = StageManager.Instance.CenterTransform.localPosition;
            Vector3 endpos = new Vector3(center.x - 0.2f, center.y, center.z);

            float journeyLength = Vector3.Distance(startpos, endpos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                SelectedModel.transform.localPosition = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedModel.transform.localPosition = endpos;
            // We always move from center to left, so no need for keeping rotation.
            //SelectedHuman.transform.rotation = StageManager.Instance.stage.transform.rotation;
        }
    }

    /// <summary>
    /// Reset.
    /// </summary>
    public void Reset() {
        // In Prius the human might not be visible; need to enable selected human and hide all organs.
        SelectedModel.SetActive(true);
        // Put the selected archetype back
        selectedArchetype.SetModelPosition();
        SelectedModel.transform.Search("BasicInfoCanvas").gameObject.SetActive(true);
        SelectedModel.GetComponentInChildren<ArchetypeInteract>().isSelected = false;
        ToggleUnselectedArchetype(true);
        archetypeSelected = false;
        selectedArchetype = null;
    }
    #endregion
}
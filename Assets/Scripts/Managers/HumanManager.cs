using System.Collections;
using UnityEngine;

/// <summary>
/// A manager that controls the human archetypes.
/// </summary>
public class HumanManager : MonoBehaviour {
    public static HumanManager Instance { get; private set; }

    public Archetype SelectedArchetype { get; set; }
    public GameObject SelectedHuman { get { return SelectedArchetype.HumanObject; } }
    public Animator HumanAnimator { get { return SelectedHuman.transform.Find("model").GetChild(0).GetComponent<Animator>(); } }
    public bool IsHumanSelected { get; set; }
    public bool StartSelectHuman { get; set; }

    private float coolingTime;
    private bool yearPanelShowed;

    #region Unity routines
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start() {
        IsHumanSelected = false;
        yearPanelShowed = false;
        coolingTime = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!IsHumanSelected) {
            if (StartSelectHuman && CheckHumanSelection()) {
                IsHumanSelected = true;
                StartSelectHuman = false;
            }
        }

        if (coolingTime < 3) {
            coolingTime += Time.deltaTime;
        }
    }
    #endregion

    #region Phase3
    /// <summary>
    /// Checks if a human model is selected.
    /// </summary>
    /// <returns><c>true</c>, if a model is selected, <c>false</c> otherwise.</returns>
    private bool CheckHumanSelection() {
        // DebugText.Instance.Log("Checking Human Selection...");
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            if (human.HumanObject.GetComponentInChildren<HumanInteract>().IsSelected) {
                SelectedArchetype = human;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Starts a coroutine to move the selected model to center of stage.
    /// </summary>
    public bool MoveSelectedHumanToCenter() {
        if (!IsHumanSelected) {
            return false;
        }

        StartCoroutine(MoveHumanTowardCenter());
        return true;
    }

    /// <summary>
    /// Moves the human toward center.
    /// </summary>
    IEnumerator MoveHumanTowardCenter() {
        if (IsHumanSelected && SelectedHuman != null) {
            float movedDist = 0;

            Vector3 startPos = SelectedHuman.transform.position;
            Vector3 endPos = StageManager.Instance.CenterTransform.position;
            print(startPos);
            print(endPos);
            float journeyLength = Vector3.Distance(startPos, endPos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                SelectedHuman.transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedHuman.transform.position = endPos;
            SelectedHuman.transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// Toggles all unselected human.
    /// </summary>
    public void ToggleUnselectedHuman(bool on) {
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            if (human.HumanObject != SelectedHuman) {
                human.HumanObject.SetActive(on);
            }
        }
    }

    /// <summary>
    /// Disables/Enables the collider of the selected human body.
    /// </summary>
    public void ToggleInteraction(bool on) {
        GameObject model = SelectedHuman.transform.Find("model").gameObject;
        model.GetComponent<CapsuleCollider>().enabled = on;
        model.GetComponent<HumanInteract>().enabled = on;
    }
    #endregion

    #region Phase4
    /// <summary>
    /// Expand selected profile details.
    /// </summary>
    public void ExpandSelectedHumanInfo() {
        if (SelectedArchetype == null || SelectedHuman == null) {
            return;
        }

        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        DetailPanelManager.Instance.ToggleDetailPanel(true);
        DetailPanelManager.Instance.SetValues();
        coolingTime = 0f;
    }

    /// <summary>
    /// The year panels are shown, but ribbons are not drawn yet.
    /// </summary>
    public void ShowYearPanels() {
        if (yearPanelShowed) {
            return;
        }

        DetailPanelManager.Instance.ToggleDetailPanel(false);
        ChoicePanelManager.Instance.ToggleChoicePanels(false);
        ButtonSequenceManager.Instance.SetPredictButton(false);
        ButtonSequenceManager.Instance.SetInfoButton(true);
        StartCoroutine(EnableYearPanels());
        ButtonSequenceManager.Instance.SetLineChartButton(true);
        TutorialManager.Instance.ShowDouble(
            LocalizationManager.Instance.FormatString("Instructions.ArchetypeInfo"),
            LocalizationManager.Instance.FormatString("Instructions.ArchetypeLineChart"),
            3.0f);
    }

    /// <summary>
    /// Hide the choice panels, shift the person to the left, and display the year panels.
    /// </summary>
    /// <returns>The year panels.</returns>
    public IEnumerator EnableYearPanels() {

        yield return MoveSelectedHumanToLeft();

        YearPanelManager.Instance.ToggleYearPanels(true);
        yield return new WaitForSeconds(2f);

        yearPanelShowed = true;
        yield return null;
    }

    /// <summary>
    /// Moves the selected human to the left of the stage.
    /// </summary>
    /// <returns><c>true</c>, if selected human to left was moved, <c>false</c> otherwise.</returns>
    public bool MoveSelectedHumanToLeft() {
        if (!IsHumanSelected) {
            return false;
        }

        StartCoroutine(MoveHumanTowardLeft());
        return true;
    }

    /// <summary>
    /// Moves the human toward left.
    /// </summary>
    IEnumerator MoveHumanTowardLeft() {
        if (IsHumanSelected && SelectedHuman != null) {
            float movedDist = 0;

            Vector3 startpos = SelectedHuman.transform.localPosition;
            Vector3 center = StageManager.Instance.CenterTransform.localPosition;
            Vector3 endpos = new Vector3(center.x - 0.2f, center.y, center.z);

            float journeyLength = Vector3.Distance(startpos, endpos);

            while (movedDist < journeyLength) {
                float fracJourney = movedDist / journeyLength;
                SelectedHuman.transform.localPosition = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedHuman.transform.localPosition = endpos;
            // We always move from center to left, so no need for keeping rotation.
            //SelectedHuman.transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// Reset.
    /// </summary>
    public void Reset() {
        yearPanelShowed = false;

        // In Prius the human might not be visible; need to enable selected human and hide all organs.
        SelectedHuman.SetActive(true);
        // Put the selected archetype back
        SelectedArchetype.SetHumanPosition();
        // Enable collider
        ToggleInteraction(true);
        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(true);
        SelectedHuman.GetComponentInChildren<HumanInteract>().IsSelected = false;
        ToggleUnselectedHuman(true);
        IsHumanSelected = false;
        SelectedArchetype = null;
    }
    #endregion
}

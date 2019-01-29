using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A manager that controls the human archetypes.
/// </summary>
public class HumanManager : MonoBehaviour {
    public static HumanManager Instance { get; private set; }

    public Archetype SelectedArchetype { get; private set; }
    public GameObject SelectedHuman { get { return SelectedArchetype.HumanObject; } }
    public bool IsHumanSelected { get; private set; }
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
            if (human.HumanObject.GetComponentInChildren<HumanInteract>().isSelected) {
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

            Vector3 startpos = SelectedHuman.transform.position;
            Vector3 endpos = StageManager.Instance.stage.transform.position;

            float journey_length = Vector3.Distance(startpos, endpos);

            while (movedDist < journey_length) {
                float fracJourney = movedDist / journey_length;
                SelectedHuman.transform.position = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedHuman.transform.position = endpos;
            SelectedHuman.transform.GetChild(1).transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// When one model is selected, hide others.
    /// </summary>
    public void HideUnselectedHuman() {
        foreach (Archetype human in ArchetypeContainer.Instance.profiles) {
            if (human.HumanObject != SelectedHuman) {
                human.HumanObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Phase4
    /// <summary>
    /// Expand selected profile details.
    /// </summary>
    /// <param name="expand">If set to <c>true</c> expand.</param>
    public void IfExpandSelectedHumanInfo(bool expand) {
        if (SelectedHuman == null) {
            return;
        }

        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(!expand);
        NumberPanelManager.Instance.ToggleDetailPanels(expand);
        coolingTime = 0f;
    }

    /// <summary>
    /// After clicking "predict", the three paths would appear.
    /// </summary>
    public void FireChoicesNextPeriod() {
        IfExpandSelectedHumanInfo(false);
        NumberPanelManager.Instance.UpdateDetailPanels();

        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        NumberPanelManager.Instance.ToggleChoicePanels(true);

        ButtonSequenceManager.Instance.SetPredictButton(false);
        TutorialText.Instance.ShowDouble("These are the paths Bodylogical generated", "Click on any panel to continue.", 5.0f);
    }

    /// <summary>
    /// The year panels are shown, but ribbons are not drawn yet.
    /// </summary>
    public void FireNextPeriod(int choice) {
        if (yearPanelShowed) {
            return;
        }

        StartCoroutine(EnableYearPanels(choice));
        ButtonSequenceManager.Instance.SetLineChartButton(true);
        TutorialText.Instance.Show("Please select \"Line Chart\" to Create Ribbon Charts.", 12.0f);
    }

    /// <summary>
    /// Hide the choice panels, shift the person to the left, and display the year panels.
    /// </summary>
    /// <returns>The year panels.</returns>
    /// <param name="choice">Choice.</param>
    public IEnumerator EnableYearPanels(int choice) {
        float animationTime = 1;

        NumberPanelManager.Instance.ToggleChoicePanels(false);
        yield return ShiftHuman(-1, animationTime);

        YearPanelManager.Instance.ToggleYearPanels(true);
        yield return new WaitForSeconds(2f);

        yearPanelShowed = true;
        yield return null;
    }

    /// <summary>
    /// Moves the selected archetype to the left of the screen.
    /// </summary>
    IEnumerator ShiftHuman(int amount, float animationTime) {
        GameObject targetHuman = SelectedHuman.transform.Search("ModelParent").gameObject;

        Vector3 des = new Vector3(targetHuman.transform.localPosition.x + amount, targetHuman.transform.localPosition.y, targetHuman.transform.localPosition.z);
        Vector3 init = targetHuman.transform.localPosition;

        float time_count = 0;
        while (time_count < animationTime) {
            targetHuman.transform.localPosition = Vector3.Lerp(init, des, time_count / animationTime);
            time_count += Time.deltaTime;
            yield return null;
        }

        targetHuman.transform.localPosition = des;

        yield return null;
    }

    /// <summary>
    /// Reset to choice panels.
    /// </summary>
    public void ResetPeriod() {
        if (!yearPanelShowed) {
            return;
        }

        StartCoroutine(ShiftHuman(1, 0.2f));

        //curr_resultPanel.SetActive(false);
        //SelectedHuman.transform.Search("ResultPanel").gameObject.SetActive(false);

        YearPanelManager.Instance.HideLines();
        YearPanelManager.Instance.ToggleYearPanels(false);
        NumberPanelManager.Instance.ToggleChoicePanels(true);

        yearPanelShowed = false;
    }
    #endregion
}

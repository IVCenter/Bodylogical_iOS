using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This is the class that manages the panels of a human
/// 
/// ***** IMPORTANT ******
/// 
/// This class is designed in a way that all the public fields will be NULL
/// until the MasterManager ask this instance to fetch values from the HumanManager
/// 
/// Because we need to wait until the HumanManager has selected the current Human
/// 
/// </summary>
public class YearPanelManager : MonoBehaviour {
    public static YearPanelManager Instance { get; private set; }

    public GameObject yearPanelParent;
    public QuadLine lineEditor;
    public ModularPanel[] yearPanels;
    public ToggleInteract[] interacts;

    private bool isCooling = false;
    private bool isBackgroundOn = true;
    private bool isDimmed = false;
    private bool isBarShown = true;

    private bool ribbonConstructed;
    private bool ribbonShown;

    #region Unity Routines
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Hide/Show the year panels.
    /// </summary>
    /// <param name="isOn">If set to <c>true</c> is on.</param>
    public void ToggleYearPanels(bool isOn) {
        // this should trigger the animations on the panels
        yearPanelParent.SetActive(isOn);
    }

    /// <summary>
    /// Loads the min/max/upper/lower bounds for each panel.
    /// </summary>
    public void LoadBounds() {
        foreach (ModularPanel panel in yearPanels) {
            panel.SetBounds();
        }
    }

    /// <summary>
    /// Loads the health values for each panel.
    /// </summary>
    public void LoadValues() {
        for (int i = 0; i < yearPanels.Length; i++) {
            yearPanels[i].SetValues(i);
        }
    }

    public void ConstructYearPanelLines() {
        lineEditor.CreateAllLines();
        ribbonConstructed = true;
        ribbonShown = true;
    }

    #endregion

    #region Alterations
    /// <summary>
    /// Hides/Shows the ribbons.
    /// </summary>
    public void ToggleRibbons() {
        if (ribbonConstructed) {
            ribbonShown = !ribbonShown;
            lineEditor.ToggleRibbons(ribbonShown);
            TutorialText.Instance.ShowDouble(
                LocalizationManager.Instance.FormatString("Instructions.LCToggleRibbon1"),
                LocalizationManager.Instance.FormatString("Instructions.LCToggleRibbon2"),
                3.0f);
        }
    }

    /// <summary>
    /// Hide/Show background.
    /// </summary>
    public void SetBackgrounds() {
        if (!ribbonConstructed) {
            return;
        }

        isBackgroundOn = !isBackgroundOn;

        if (isBackgroundOn == false) {
            TutorialText.Instance.Show(LocalizationManager.Instance.FormatString("Instructions.LCBackground"), 4.5f);
        }

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleAllBackground(isBackgroundOn);
        }
    }

    /// <summary>
    /// Dim bar color.
    /// </summary>
    public void DimAllBars() {
        bool toDim = !isDimmed;

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleColor(toDim);
        }

        if (toDim) {
            TutorialText.Instance.ShowDouble(
                LocalizationManager.Instance.FormatString("Instructions.LCDim1"),
                LocalizationManager.Instance.FormatString("Instructions.LCDim2"),
                3.8f);
        }

        isDimmed = toDim;
    }

    /// <summary>
    /// Hide bar color.
    /// </summary>
    public void SetAllBars() {
        if (!ribbonConstructed) {
            return;
        }

        isBarShown = !isBarShown;

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleAllBars(isBarShown);
        }

        if (!isBarShown) {
            TutorialText.Instance.ShowDouble(
                LocalizationManager.Instance.FormatString("Instructions.LCSet1"),
                LocalizationManager.Instance.FormatString("Instructions.LCSet2"),
                4.5f);
        }
    }
    #endregion

    #region Pull Right
    public void PullOverall(bool isOn) {
        PullBioMetrics(HealthType.overall, isOn);
    }

    public void PullBodyFat(bool isOn) {
        PullBioMetrics(HealthType.bodyFatMass, isOn);
    }

    public void PullBMI(bool isOn) {
        PullBioMetrics(HealthType.bmi, isOn);
    }

    public void PullAIC(bool isOn) {
        PullBioMetrics(HealthType.aic, isOn);
    }

    public void PullLDL(bool isOn) {
        PullBioMetrics(HealthType.ldl, isOn);
    }

    public void PullSBP(bool isOn) {
        PullBioMetrics(HealthType.sbp, isOn);
    }

    /// <summary>
    /// Hide/Show a specific biometric.
    /// </summary>
    /// <param name="type">type of the biometric.</param>
    public void PullBioMetrics(HealthType type, bool isOn) {
        if (isCooling) {
            TutorialText.Instance.ShowDouble(
                LocalizationManager.Instance.FormatString("Instructions.LCPullError1"),
                LocalizationManager.Instance.FormatString("Instructions.LCPullError1"),
                2.5f);
            interacts[ModularPanel.typeSectionDictionary[type]].Toggle();
            return;
        }

        StartCoroutine(Cooling());

        foreach (ModularPanel panel in yearPanels) {
            StartCoroutine(panel.PullSection(ModularPanel.typeSectionDictionary[type], isOn));
        }

        if (!isBackgroundOn && isOn) {
            TutorialText.Instance.ShowDouble(
                LocalizationManager.Instance.FormatString("Instructions.LCPull1"),
                LocalizationManager.Instance.FormatString("Instructions.LCPull2"),
                5.5f);
        }
    }

    IEnumerator Cooling() {
        isCooling = true;
        yield return new WaitForSeconds(3f);

        isCooling = false;
        yield return null;
    }
    #endregion

    #region LineChartVisualization
    /// <summary>
    /// Toggles the line chart.
    /// </summary>
    public void ToggleLineChart(bool on) {
        ButtonSequenceManager.Instance.SetLineChartButton(!on);

        ButtonSequenceManager.Instance.SetLineChartFunction(on);
        ButtonSequenceManager.Instance.SetActivitiesButton(on);
        ButtonSequenceManager.Instance.SetPriusButton(on);
    }

    /// <summary>
    /// If the ribbon charts are not drawn, draw the ribbons across the year panels.
    /// </summary>
    public IEnumerator StartLineChart(GameObject orig) {
        if (!ribbonConstructed) {
            ConstructYearPanelLines();
        } else {
            yield return StageManager.Instance.ChangeVisualization(orig, yearPanelParent);
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        TutorialText.Instance.Show(LocalizationManager.Instance.FormatString("Instructions.LCAlter"), 3);
        yield return null;
    }

    public void Reset() {
        lineEditor.ResetLines();
        ribbonConstructed = false;
    }
    #endregion
}
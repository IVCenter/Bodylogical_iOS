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

    public GameObject parent;
    public QuadLine lineEditor;
    public ModularPanel[] yearPanels;
    public ToggleInteract[] interacts;

    private bool isCooling = false;
    private bool isBackgroundOn = true;
    private bool isSeparated = false; // NOT USED
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
        parent.SetActive(isOn);
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
            TutorialText.Instance.ShowDouble("You have toggled the ribbon charts", "This would allow you to focus on individual years", 3.0f);
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

        if (isBackgroundOn == false && isSeparated == false) {
            TutorialText.Instance.Show("You just hide the light blue panel background.", 4.5f);
        }

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleAllBackground(isBackgroundOn);
        }
    }

    /// <summary>
    /// Separates the panels.
    /// CURRENTLY REMOVED FROM APP.
    /// </summary>
    public void SeparatePanels() {
        if (isCooling) {
            TutorialText.Instance.ShowDouble("You clicked too fast.", "Wait a second and try \"Split\" button again", 2.5f);
            return;
        }

        StartCoroutine(Cooling());

        isSeparated = !isSeparated;

        foreach (ModularPanel panel in yearPanels) {
            panel.SeparateSections(isSeparated);
        }

        if (!isBackgroundOn && isSeparated) {
            TutorialText.Instance.ShowDouble("You just splitted items into two groups, ", "Click again to move them back", 5.5f);
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
            TutorialText.Instance.ShowDouble("You just dimmed the bars,", "Click again to light them up", 3.8f);
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
            TutorialText.Instance.ShowDouble("You just hid the bars,", "Click again to show them.", 4.5f);
        }
    }
    #endregion

    #region Toggle CURRENTLY REMOVED FROM APP
    public void ToggleOverall(bool isOn) {
        ToggleBioMetrics(HealthType.overall, isOn);
    }

    public void ToggleBodyFat(bool isOn) {
        ToggleBioMetrics(HealthType.bodyFatMass, isOn);
    }

    public void ToggleBMI(bool isOn) {
        ToggleBioMetrics(HealthType.bmi, isOn);
    }

    public void ToggleAIC(bool isOn) {
        ToggleBioMetrics(HealthType.aic, isOn);
    }

    public void ToggleLDL(bool isOn) {
        ToggleBioMetrics(HealthType.ldl, isOn);
    }

    public void ToggleSBP(bool isOn) {
        ToggleBioMetrics(HealthType.sbp, isOn);
    }

    /// <summary>
    /// Hide/Show a specific biometric.
    /// </summary>
    /// <param name="type">type of the biometric.</param>
    public void ToggleBioMetrics(HealthType type, bool isOn) {
        foreach (ModularPanel panel in yearPanels) {
            panel.Toggle(ModularPanel.typeSectionDictionary[type], isOn);
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
            TutorialText.Instance.ShowDouble("You clicked too fast.", "Wait a second and try again.", 2.5f);
            interacts[ModularPanel.typeSectionDictionary[type]].Toggle();
            return;
        }

        StartCoroutine(Cooling());

        foreach (ModularPanel panel in yearPanels) {
            StartCoroutine(panel.PullSection(ModularPanel.typeSectionDictionary[type], isOn));
        }

        if (!isBackgroundOn && isOn) {
            TutorialText.Instance.ShowDouble("You just pull the section to the right,", "Click again to move it back,", 5.5f);
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

        ToggleYearPanels(on);
        ButtonSequenceManager.Instance.SetLineChartFunction(on);

        ButtonSequenceManager.Instance.SetActivitiesButton(on);
        ButtonSequenceManager.Instance.SetPriusButton(on);
    }

    /// <summary>
    /// If the ribbon charts are not drawn, draw the ribbons across the year panels.
    /// </summary>
    public IEnumerator StartLineChart() {
        if (!ribbonConstructed) {
            ConstructYearPanelLines();
        } else {
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        HumanManager.Instance.ModelTransform.localPosition = new Vector3(0, 0, 0);
        TutorialText.Instance.ShowDouble("Use the left buttons to toggle biometrics", "Use the right button to alter the ribbon charts", 3);
        yield return null;    
    }

    public void Reset() {
        lineEditor.ResetLines();
        ribbonConstructed = false;
    }
    #endregion
}
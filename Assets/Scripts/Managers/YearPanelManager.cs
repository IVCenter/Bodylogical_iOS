using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool ribbonConstructed;

    private Dictionary<HealthType, bool> pulled = new Dictionary<HealthType, bool> {
        { HealthType.overall, false },
        { HealthType.bodyFatMass, false },
        { HealthType.bmi, false },
        { HealthType.aic, false },
        { HealthType.ldl, false },
        { HealthType.sbp, false }
    };
    private Dictionary<HealthType, bool> cooling = new Dictionary<HealthType, bool> {
        { HealthType.overall, false },
        { HealthType.bodyFatMass, false },
        { HealthType.bmi, false },
        { HealthType.aic, false },
        { HealthType.ldl, false },
        { HealthType.sbp, false }
    };

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
    }

    #endregion

    #region LineChartVisualization
    /// <summary>
    /// Toggles the line chart.
    /// </summary>
    public void ToggleLineChart(bool on) {
        ControlPanelManager.Instance.ToggleLineChartSelector(on);
        ControlPanelManager.Instance.ToggleLineChartControls(on);
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
        TutorialManager.Instance.ShowStatus("Instructions.LCAlter");
        yield return null;
    }

    public void Reset() {
        lineEditor.ResetLines();
        ribbonConstructed = false;
    }
    #endregion

    #region Alterations
    /// <summary>
    /// Hides/Shows the ribbons.
    /// </summary>
    public void ToggleRibbons(bool on) {
        if (ribbonConstructed) {
            lineEditor.ToggleRibbons(on);
            TutorialManager.Instance.ShowStatus("Instructions.LCToggleRibbon");
        }
    }

    /// <summary>
    /// Hide/Show background.
    /// </summary>
    public void ToggleBackgroundTransparency(bool on) {
        if (!ribbonConstructed) {
            return;
        }

        if (!on) {
            TutorialManager.Instance.ShowStatus("Instructions.LCBackground");
        }

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleAllBackground(on);
        }
    }

    /// <summary>
    /// Dim bar color.
    /// </summary>
    public void DimBarColors(bool on) {
        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleColor(!on);
        }

        if (!on) {
            TutorialManager.Instance.ShowStatus("Instructions.LCDim");
        }

    }

    /// <summary>
    /// Hide bar color.
    /// </summary>
    public void ToggleBarTransparency(bool on) {
        if (!ribbonConstructed) {
            return;
        }

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleAllBars(on);
        }

        if (!on) {
            TutorialManager.Instance.ShowStatus("Instructions.LCSet");
        }
    }

    /// <summary>
    /// Hide/Show a specific biometric.
    /// </summary>
    /// <param name="index">index of the biometric.</param>
    public void PullBioMetrics(int index) {
        HealthType type = (HealthType)index;
        if (cooling[type]) {
            TutorialManager.Instance.ShowStatus("Instructions.LCPullError");
            return;
        }

        pulled[type] = !pulled[type];

        StartCoroutine(Cooling(type));

        foreach (ModularPanel panel in yearPanels) {
            StartCoroutine(panel.PullSection(ModularPanel.typeSectionDictionary[type], pulled[type]));
        }

        if (pulled[type]) {
            TutorialManager.Instance.ShowStatus("Instructions.LCPull");
        }
    }

    IEnumerator Cooling(HealthType type) {
        cooling[type] = true;
        yield return new WaitForSeconds(2.0f);

        cooling[type] = false;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the controls for the line charts.
/// </summary>
public class LineChartManager : MonoBehaviour {
    public static LineChartManager Instance { get; private set; }

    public GameObject yearPanelParent;
    [SerializeField] private Ribbons lineEditor;
    public ModularPanel[] yearPanels;

    [SerializeField] private Color nonePointer, minimalPointer, optimalPointer;
    [SerializeField] private Material noneRibbon, minimalRibbon, optimalRibbon;

    private bool ribbonConstructed;

    private HealthType? highlighted = null;

    private bool cooling = false;

    private Dictionary<HealthChoice, Color> pointers;
    private Dictionary<HealthChoice, Material> ribbons;

    [SerializeField] private Transform lineChartTutorialTransform;
    [HideInInspector] public bool tutorialShwon;

    #region Unity Routines
    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        pointers = new Dictionary<HealthChoice, Color> {
            { HealthChoice.None, nonePointer },
            { HealthChoice.Minimal, minimalPointer },
            { HealthChoice.Optimal, optimalPointer }
        };

        ribbons = new Dictionary<HealthChoice, Material> {
            { HealthChoice.None, noneRibbon },
            { HealthChoice.Minimal, minimalRibbon },
            { HealthChoice.Optimal, optimalRibbon }
        };
    }
    #endregion

    #region Initialization
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
            yearPanels[i].SetValues(i, TimeProgressManager.Instance.Path);
        }
    }

    public void ConstructYearPanelLines() {
        HealthChoice path = TimeProgressManager.Instance.Path;
        lineEditor.CreateAllLines(pointers[path], ribbons[path]);
        ribbonConstructed = true;
    }

    public void Reload() {
        lineEditor.ResetLines();
        LoadValues();
        ConstructYearPanelLines();
    }
    #endregion

    #region LineChartVisualization
    /// <summary>
    /// Toggles the line chart.
    /// </summary>
    public void ToggleLineChart(bool on) {
        ControlPanelManager.Instance.ToggleLineChartSelector(on);
        ControlPanelManager.Instance.ToggleLineChartControls(on);
        ChoicePanelManager.Instance.ToggleChoicePanels(on);
        ChoicePanelManager.Instance.SetValues();
    }

    /// <summary>
    /// If the ribbon charts are not drawn, draw the ribbons across the year panels.
    /// </summary>
    public IEnumerator StartLineChart(GameObject orig) {
        if (!ribbonConstructed) {
            ConstructYearPanelLines();
        }
        yield return StageManager.Instance.ChangeVisualization(orig, yearPanelParent);
        yield return ArchetypeManager.Instance.MoveSelectedArchetypeToLeft();
        TutorialManager.Instance.ShowStatus("Instructions.LCAlter");

        if (!tutorialShwon) {
            TutorialManager.Instance.ClearTutorial();
            TutorialParam param = new TutorialParam("Tutorials.LCIntroTitle", "Tutorials.LCIntroText");
            TutorialManager.Instance.ShowTutorial(param, lineChartTutorialTransform);
            tutorialShwon = true;
        }
    }

    public void Reset() {
        lineEditor.ResetLines();
        ribbonConstructed = false;
        yearPanelParent.SetActive(false);
    }
    #endregion

    #region Alterations
    /// <summary>
    /// Hides/Shows the ribbons.
    /// </summary>
    /// <param name="on">If <see langword="true"/>, the ribbons are displayed.</param>
    public void ToggleRibbons(bool on) {
        if (ribbonConstructed) {
            lineEditor.ToggleRibbons(on);
            TutorialManager.Instance.ShowStatus("Instructions.LCToggleRibbon");
        }
    }

    /// <summary>
    /// Hide/Show background.
    /// </summary>
    /// <param name="on">If <see langword="true"/>, set transparent.</param>
    public void ToggleBackgroundTransparency(bool on) {
        if (ribbonConstructed) {
            if (on) {
                TutorialManager.Instance.ShowStatus("Instructions.LCBackground");
            }

            foreach (ModularPanel panel in yearPanels) {
                panel.ToggleAllBackground(on);
            }
        }
    }

    /// <summary>
    /// Dim bar color.
    /// </summary>
    /// <param name="on">If <see langword="true"/>, dim the colors.</param>
    public void DimBarColors(bool on) {
        if (ribbonConstructed) {
            foreach (ModularPanel panel in yearPanels) {
                panel.ToggleColor(on);
            }

            if (on) {
                TutorialManager.Instance.ShowStatus("Instructions.LCDim");
            }
        }
    }

    /// <summary>
    /// Hide bar color.
    /// </summary>
    /// <param name="on">If <see langword="true"/>, hide bar colors.</param>
    public void ToggleBarTransparency(bool on) {
        if (ribbonConstructed) {
            foreach (ModularPanel panel in yearPanels) {
                panel.ToggleAllBars(on);
            }

            if (on) {
                TutorialManager.Instance.ShowStatus("Instructions.LCSet");
            }
        }
    }

    /// <summary>
    /// Separate a specific biometric.
    /// </summary>
    /// <param name="index">index of the biometric.</param>
    public void PullBioMetrics(int index) {
        HealthType type = (HealthType)index;
        if (cooling) {
            TutorialManager.Instance.ShowStatus("Instructions.LCPullError");
            return;
        }

        StartCoroutine(Cooling());
        foreach (ModularPanel panel in yearPanels) {
            StartCoroutine(panel.PullSection(type));
        }

        highlighted = highlighted == type ? null : (HealthType?)type;

        if (highlighted != null) {
            TutorialManager.Instance.ShowStatus("Instructions.LCPull");
        }
    }

    private IEnumerator Cooling() {
        cooling = true;
        yield return new WaitForSeconds(2.0f);
        cooling = false;
    }
    #endregion
}
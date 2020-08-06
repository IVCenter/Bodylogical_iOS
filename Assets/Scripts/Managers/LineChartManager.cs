using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the controls for the line charts.
/// </summary>
public class LineChartManager : MonoBehaviour {
    public static LineChartManager Instance { get; private set; }

    public GameObject yearPanelParent;
    [SerializeField] private Ribbons lineEditor;
    public ModularPanel[] yearPanels;
    [SerializeField] private ColorLibrary colorLibrary;
    [SerializeField] private Material noneRibbon, minimalRibbon, optimalRibbon;

    private bool ribbonConstructed;
    private HealthType? highlighted;
    private bool cooling;
    private Dictionary<HealthChoice, Material> ribbons;

    // The following variables are for tutorial purposes.
    [SerializeField] private Transform lineChartTutorialTransform;
    [SerializeField] private Transform panelTutorialTransform;

    public bool TutorialShown { get; set; }
    private bool ribbonPanelClicked;

    #region Unity Routines

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        ribbons = new Dictionary<HealthChoice, Material> {
            {HealthChoice.None, noneRibbon},
            {HealthChoice.Minimal, minimalRibbon},
            {HealthChoice.Optimal, optimalRibbon}
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

    public void ConstructLineChart() {
        HealthChoice path = TimeProgressManager.Instance.Path;
        lineEditor.CreateAllLines(colorLibrary.ChoiceColorDict[path], ribbons[path]);
        ribbonConstructed = true;
    }

    public void Reload() {
        lineEditor.ResetLines();
        LoadValues();
        ConstructLineChart();
    }

    #endregion

    #region LineChartVisualization

    /// <summary>
    /// Toggles the line chart.
    /// </summary>
    public void ToggleLineChart(bool on) {
        ChoicePanelManager.Instance.ToggleChoicePanels(on);
        ChoicePanelManager.Instance.SetValues();
        ControlPanelManager.Instance.ToggleRibbonAccess(on);
    }

    /// <summary>
    /// If the ribbon charts are not drawn, draw the ribbons across the year panels.
    /// </summary>
    public IEnumerator StartLineChart(GameObject orig) {
        if (!ribbonConstructed) {
            ConstructLineChart();
        }

        yield return StageManager.Instance.ChangeVisualization(orig, yearPanelParent);

        if (!TutorialShown) {
            TutorialParam param = new TutorialParam("Tutorials.RibbonTitle", "Tutorials.RibbonText");
            TutorialManager.Instance.ShowTutorial(param, lineChartTutorialTransform, () => highlighted != null,
                postCallback: ShowTutPull);
            TutorialShown = true;
        }
    }

    public void Reset() {
        lineEditor.ResetLines();
        ribbonConstructed = false;
        yearPanelParent.SetActive(false);
    }

    #endregion

    # region Tutorials

    private void ShowTutPull() {
        TutorialParam param = new TutorialParam("Tutorials.RibbonTitle", "Tutorials.RibbonText2");
        TutorialManager.Instance.ShowTutorial(param, lineChartTutorialTransform, () => ribbonPanelClicked,
            () => ribbonPanelClicked = false, ShowTutPanel);
    }

    public void RibbonClicked() {
        ribbonPanelClicked = true;
    }

    private void ShowTutPanel() {
        TutorialParam param = new TutorialParam("Tutorials.RPanelTitle", "Tutorials.RPanelText");
        TutorialManager.Instance.ShowTutorial(param, panelTutorialTransform,
            () => !ribbonsOn, postCallback: ShowTutPanel2);
    }

    private void ShowTutPanel2() {
        TutorialParam param = new TutorialParam("Tutorials.RPanelTitle", "Tutorials.RPanelText2");
        TutorialManager.Instance.ShowTutorial(param, panelTutorialTransform,
            () => backgroundTransparent, postCallback: ShowTutPanel3);
    }

    private void ShowTutPanel3() {
        TutorialParam param = new TutorialParam("Tutorials.RPanelTitle", "Tutorials.RPanelText3");
        TutorialManager.Instance.ShowTutorial(param, panelTutorialTransform,
            () => colorsDim, postCallback: ShowTutPanel4);
    }

    private void ShowTutPanel4() {
        TutorialParam param = new TutorialParam("Tutorials.RPanelTitle", "Tutorials.RPanelText4");
        TutorialManager.Instance.ShowTutorial(param, panelTutorialTransform,
            () => barTransparent, postCallback: ShowTutPanel5);
    }

    private void ShowTutPanel5() {
        TutorialParam param = new TutorialParam("Tutorials.RPanelTitle", "Tutorials.RPanelText5");
        TutorialManager.Instance.ShowTutorial(param, panelTutorialTransform,
            () => AppStateManager.Instance.CurrState != AppState.VisLineChart);
    }

    # endregion

    #region Alterations

    private bool ribbonsOn = true;
    private bool backgroundTransparent;
    private bool colorsDim;
    private bool barTransparent;

    /// <summary>
    /// Hides/Shows the ribbons.
    /// </summary>
    public void ToggleRibbons() {
        if (ribbonConstructed) {
            ribbonsOn = !ribbonsOn;
            lineEditor.ToggleRibbons(ribbonsOn);
            TutorialManager.Instance.ShowStatus("Instructions.LCToggleRibbon");
        }
    }

    /// <summary>
    /// Hide/Show background.
    /// </summary>
    public void ToggleBackgroundTransparency() {
        if (ribbonConstructed) {
            backgroundTransparent = !backgroundTransparent;
            if (backgroundTransparent) {
                TutorialManager.Instance.ShowStatus("Instructions.LCBackground");
            }

            foreach (ModularPanel panel in yearPanels) {
                panel.ToggleAllBackground(backgroundTransparent);
            }
        }
    }

    /// <summary>
    /// Dim bar color.
    /// </summary>
    public void DimBarColors() {
        if (ribbonConstructed) {
            colorsDim = !colorsDim;

            foreach (ModularPanel panel in yearPanels) {
                panel.ToggleColor(colorsDim);
            }

            if (colorsDim) {
                TutorialManager.Instance.ShowStatus("Instructions.LCDim");
            }
        }
    }

    /// <summary>
    /// Hide bar color.
    /// </summary>
    public void ToggleBarTransparency() {
        if (ribbonConstructed) {
            barTransparent = !barTransparent;

            foreach (ModularPanel panel in yearPanels) {
                panel.ToggleAllBars(barTransparent);
            }

            if (barTransparent) {
                TutorialManager.Instance.ShowStatus("Instructions.LCSet");
            }
        }
    }

    /// <summary>
    /// Separate a specific biometric.
    /// </summary>
    /// <param name="index">index of the biometric.</param>
    public void PullBioMetrics(int index) {
        HealthType type = (HealthType) index;
        if (cooling) {
            TutorialManager.Instance.ShowStatus("Instructions.LCPullError");
            return;
        }

        StartCoroutine(Cooling());
        foreach (ModularPanel panel in yearPanels) {
            StartCoroutine(panel.PullSection(type));
        }

        highlighted = highlighted == type ? null : (HealthType?) type;

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
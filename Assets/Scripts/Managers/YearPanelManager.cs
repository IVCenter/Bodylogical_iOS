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
    public GameObject lineEditor;
    public ModularPanel[] yearPanels;

    private readonly Dictionary<HealthType, int> biometricIndexDict = new Dictionary<HealthType, int>() {
        { HealthType.overall, 0 },
        { HealthType.bodyFatMass, 1 },
        { HealthType.bmi, 2 },
        { HealthType.aic, 3 },
        { HealthType.ldl, 4 },
        { HealthType.sbp, 5 }
    };

    private bool lineCreated = false;

    private bool isCooling = false;
    private bool isBackgroundOn = true;
    private bool isSeparated = false;
    private bool isDimmed = false;
    private bool isBarShown = true;

    private bool ribbonConstructed;
    private bool ribbonShown;

    private QuadLine Lines { get { return lineEditor.GetComponent<QuadLine>(); } }
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
        if (MasterManager.Instance.CurrGamePhase != MasterManager.GamePhase.Interaction) {
            DebugText.Instance.Log("Cannot maniplate year panel if not in Phase5");
            return;
        }

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
        if (lineCreated) {
            lineEditor.SetActive(true);
            return;
        }

        if (MasterManager.Instance.CurrGamePhase != MasterManager.GamePhase.Interaction) {
            DebugText.Instance.Log("Cannot maniplate year panel if not in Phase5");
            return;
        }

        lineCreated = true;

        Lines.CreateAllLines();
        ribbonShown = true;
    }

    public void ToggleRibbons() {
        if (lineCreated) {
            ribbonShown = !ribbonShown;
            lineEditor.SetActive(ribbonShown);
        }
    }

    public void Reset() {
        Lines.ResetLines();
    }
    #endregion

    #region Alterations
    /// <summary>
    /// Hide/Show background.
    /// </summary>
    public void SetBackgrounds(bool isOn) {
        if (!lineCreated) {
            return;
        }

        isBackgroundOn = !isBackgroundOn;

        if (isBackgroundOn == false && isSeparated == false) {
            TutorialText.Instance.Show("You just hide the light blue panel background, ", 4.5f);
        }

        if (isBackgroundOn && isSeparated) {
            TutorialText.Instance.ShowDouble("The light blue panel might look messed up, ", "Please stay \"Transaprent\" in Split Mode.", 5.0f);
        }

        foreach (ModularPanel panel in yearPanels) {
            panel.ToggleAllBackground(isBackgroundOn);
        }
    }

    /// <summary>
    /// Separates the panels.
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

        if (isBackgroundOn && isSeparated) {
            TutorialText.Instance.ShowDouble("If the Light Blue panel looks messy, ", "Click \"Transaprent\" to hide it.", 5.0f);
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
        if (!lineCreated) {
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

    #region Toggle
    /// <summary>
    /// Hide/Show a specific biometric.
    /// </summary>
    /// <param name="name">type of the biometric.</param>
    public void ToggleBioMetrics(string name) {
        string status = "OFF"; // indicates if the metric is turned off or on.

        HealthType type = (HealthType)System.Enum.Parse(typeof(HealthType), name);
        foreach (ModularPanel panel in yearPanels) {
            bool toggleStatus = panel.Toggle(ModularPanel.typeSectionDictionary[type]);

            if (toggleStatus) {
                status = "ON";
            }
        }

        int buttonIndex = biometricIndexDict[type];

        string text = ButtonSequenceManager.Instance.toggleButtons[buttonIndex].transform.Search("Text").GetComponent<Text>().text;
        string output = text.Split(':')[0];
        output = string.Concat(output, ":");
        output = string.Concat(output, status);
        ButtonSequenceManager.Instance.toggleButtons[buttonIndex].transform.Search("Text").GetComponent<Text>().text = output;
    }
    #endregion

    #region LineChartVisualization
    /// <summary>
    /// Toggles the line chart.
    /// </summary>
    public void ToggleLineChart(bool on) {
        ButtonSequenceManager.Instance.SetLineChartButton(!on);

        ToggleYearPanels(on);
        ButtonSequenceManager.Instance.SetToggleButtons(on);
        ButtonSequenceManager.Instance.SetFunctionButtons(on);

        ButtonSequenceManager.Instance.SetPropsButton(on);
        ButtonSequenceManager.Instance.SetInternals(on);
    }

    /// <summary>
    /// If the ribbon charts are not drawn, draw the ribbons across the year panels.
    /// </summary>
    public IEnumerator StartLineChart() {
        if (!ribbonConstructed) {
            ribbonConstructed = true;
            ConstructYearPanelLines();
        } else {
            yield return HumanManager.Instance.MoveSelectedHumanToLeft();
        }
        TutorialText.Instance.ShowDouble("Use the left buttons to toggle biometrics", "Use the right button to alter the ribbon charts", 3);
        yield return null;    
    }
    #endregion

    #region Alteration Helpers
    IEnumerator Cooling() {
        isCooling = true;
        yield return new WaitForSeconds(3f);

        isCooling = false;
        yield return null;
    }
    #endregion
}
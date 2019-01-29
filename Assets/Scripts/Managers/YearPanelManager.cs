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

    public Color darkBlue;
    public Color darkYellow;
    public Color darkRed;

    public GameObject parent;
    public GameObject year0panel;
    public GameObject year1panel;
    public GameObject year2panel;
    public GameObject year3panel;
    public GameObject year4panel;
    public GameObject lineEditor;

    private List<GameObject> AllPanels = new List<GameObject>();
    private List<GameObject> overallPanels = new List<GameObject>();
    private List<GameObject> bodyFatPanels = new List<GameObject>();
    private List<GameObject> bmiPanels = new List<GameObject>();
    private List<GameObject> hba1cPanels = new List<GameObject>();
    private List<GameObject> ldlPanels = new List<GameObject>();
    private List<GameObject> bpPanels = new List<GameObject>();

    private List<Image> allpanelBackgrounds = new List<Image>();
    private List<Image> normalBars = new List<Image>();
    private List<Image> warningBars = new List<Image>();
    private List<Image> upperBars = new List<Image>();

    private List<Color> colorCache = new List<Color>();

    private readonly Dictionary<string, int> biometricIndexDict = new Dictionary<string, int>() {
        { "Overall Panel", 0 },
        { "Body Fat Panel", 1 },
        { "BMI Panel", 2 },
        { "HbA1c Panel", 3 },
        { "LDL Panel", 4 },
        { "Blood Pressure Panel", 5 }
    };

    private bool lineCreated = false;

    private bool isCooling = false;
    private bool isBackgroundOn = true;
    private bool isSeparated = false;
    private bool isDimmed = false;
    private bool isBarShown = true;

    private bool ribbonConstructed;

    #region Unity Routines
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    void Start() {
        lineCreated = false;

        AllPanels.Add(year0panel);
        AllPanels.Add(year1panel);
        AllPanels.Add(year2panel);
        AllPanels.Add(year3panel);
        AllPanels.Add(year4panel);

        ParseAllPanelsAndBars();
    }

    #endregion

    #region Initialization
    private void ParseAllPanelsAndBars() {
        foreach (GameObject yearpanel in AllPanels) {
            GameObject overallpanel = yearpanel.transform.Search("Overall Panel").gameObject;
            GameObject bodyfatpanel = yearpanel.transform.Search("Body Fat Panel").gameObject;
            GameObject bmipanel = yearpanel.transform.Search("BMI Panel").gameObject;
            GameObject hba1cpanel = yearpanel.transform.Search("HbA1c Panel").gameObject;
            GameObject ldlpanel = yearpanel.transform.Search("LDL Panel").gameObject;
            GameObject bppanel = yearpanel.transform.Search("Blood Pressure Panel").gameObject;

            overallPanels.Add(overallpanel);
            bodyFatPanels.Add(bodyfatpanel);
            bmiPanels.Add(bmipanel);
            hba1cPanels.Add(hba1cpanel);
            ldlPanels.Add(ldlpanel);
            bpPanels.Add(bppanel);

            allpanelBackgrounds.Add(overallpanel.GetComponent<Image>());
            allpanelBackgrounds.Add(bodyfatpanel.GetComponent<Image>());
            allpanelBackgrounds.Add(bmipanel.GetComponent<Image>());
            allpanelBackgrounds.Add(hba1cpanel.GetComponent<Image>());
            allpanelBackgrounds.Add(ldlpanel.GetComponent<Image>());
            allpanelBackgrounds.Add(bppanel.GetComponent<Image>());

            normalBars.Add(overallpanel.transform.Search("Normal").GetComponent<Image>());
            warningBars.Add(overallpanel.transform.Search("Warning").GetComponent<Image>());
            upperBars.Add(overallpanel.transform.Search("Upper").GetComponent<Image>());

            normalBars.Add(bodyfatpanel.transform.Search("Normal").GetComponent<Image>());
            warningBars.Add(bodyfatpanel.transform.Search("Warning").GetComponent<Image>());
            upperBars.Add(bodyfatpanel.transform.Search("Upper").GetComponent<Image>());

            normalBars.Add(bmipanel.transform.Search("Normal").GetComponent<Image>());
            warningBars.Add(bmipanel.transform.Search("Warning").GetComponent<Image>());
            upperBars.Add(bmipanel.transform.Search("Upper").GetComponent<Image>());

            normalBars.Add(hba1cpanel.transform.Search("Normal").GetComponent<Image>());
            warningBars.Add(hba1cpanel.transform.Search("Warning").GetComponent<Image>());
            upperBars.Add(hba1cpanel.transform.Search("Upper").GetComponent<Image>());

            normalBars.Add(ldlpanel.transform.Search("Normal").GetComponent<Image>());
            warningBars.Add(ldlpanel.transform.Search("Warning").GetComponent<Image>());
            upperBars.Add(ldlpanel.transform.Search("Upper").GetComponent<Image>());

            normalBars.Add(bppanel.transform.Search("Normal").GetComponent<Image>());
            warningBars.Add(bppanel.transform.Search("Warning").GetComponent<Image>());
            upperBars.Add(bppanel.transform.Search("Upper").GetComponent<Image>());
        }
    }

    /// <summary>
    /// Hide/Show the year panels.
    /// </summary>
    /// <param name="isOn">If set to <c>true</c> is on.</param>
    public void ToggleYearPanels(bool isOn) {
        if (MasterManager.Instance.CurrGamePhase != MasterManager.GamePhase.Phase5) {
            DebugText.Instance.Log("Cannot maniplate year panel if not in Phase5");
            return;
        }

        // this should trigger the animations on the panels
        parent.SetActive(isOn);
    }

    public void ConstructYearPanelLines() {
        if (lineCreated) {
            lineEditor.SetActive(true);
            return;
        }

        if (MasterManager.Instance.CurrGamePhase != MasterManager.GamePhase.Phase5) {
            DebugText.Instance.Log("Cannot maniplate year panel if not in Phase5");
            return;
        }

        lineCreated = true;

        lineEditor.GetComponent<QuadLine>().CreateAllLines();
    }

    public void HideLines() {
        if (lineCreated) {
            lineEditor.SetActive(false);
        }
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

        foreach (Image img in allpanelBackgrounds) {
            img.enabled = isBackgroundOn;
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

        if (isSeparated) {
            //print("Separating panels...");
            MovePanels(overallPanels, false, false);
            MovePanels(bmiPanels, false, false);
            MovePanels(ldlPanels, false, false);
            MovePanels(bodyFatPanels, true, false);
            MovePanels(hba1cPanels, true, false);
            MovePanels(bpPanels, true, false);
        } else {
            MovePanels(overallPanels, false, true);
            MovePanels(bmiPanels, false, true);
            MovePanels(ldlPanels, false, true);
            MovePanels(bodyFatPanels, true, true);
            MovePanels(hba1cPanels, true, true);
            MovePanels(bpPanels, true, true);
        }

        if (isBackgroundOn && isSeparated) {
            TutorialText.Instance.ShowDouble("If the Light Blue panel looks a little messed up, ", "Click \"Transaprent\" to hide it.", 5.0f);
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

        if (toDim) {
            colorCache.Add(DarkenBarColor(normalBars, darkBlue, toDim));
            colorCache.Add(DarkenBarColor(warningBars, darkYellow, toDim));
            colorCache.Add(DarkenBarColor(upperBars, darkRed, toDim));
        } else {
            DarkenBarColor(normalBars, colorCache[0], toDim);
            DarkenBarColor(warningBars, colorCache[1], toDim);
            DarkenBarColor(upperBars, colorCache[2], toDim);
            colorCache.Clear();
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
        isBarShown = !isBarShown;

        SetBars(normalBars, isBarShown);
        SetBars(warningBars, isBarShown);
        SetBars(upperBars, isBarShown);

        if (!isBarShown) {
            TutorialText.Instance.ShowDouble("You just hid the bars,", "Click again to show them.", 4.5f);
        }
    }
    #endregion

    #region Toggle
    /// <summary>
    /// Hide/Show a specific biometric.
    /// </summary>
    /// <param name="name">name of the biometric.</param>
    public void ToggleBioMetrics(string name) {
        string status = "OFF"; // indicates if the metric is turned off or on.

        foreach (GameObject panel in AllPanels) {
            GameObject target = panel.transform.Search(name).gameObject;
            target.SetActive(!target.activeSelf);

            if (target.activeSelf) {
                status = "ON";
            }
        }

        int buttonIndex = biometricIndexDict[name];

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
    }

    /// <summary>
    /// If the ribbon charts are not drawn, rraw the ribbons across the year panels.
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

    private void MovePanels(List<GameObject> currPanels, bool isLeft, bool isGoBack) {
        if (!lineCreated) {
            return;
        }

        List<RectTransform> transList = new List<RectTransform>();

        foreach (GameObject panel in currPanels) {
            transList.Add(panel.GetComponent<RectTransform>());
        }

        StartCoroutine(MovePanelHelper(transList, isLeft, isGoBack));
    }

    IEnumerator MovePanelHelper(List<RectTransform> transList, bool isLeft, bool isGoBack) {
        float endLeft, endRight;
        float animationTime = 2.0f;

        if (isLeft) {
            endLeft = -400f;
            endRight = 400f;
        } else {
            endLeft = 400f;
            endRight = -400f;
        }

        if (isGoBack) {
            endLeft = 0f;
            endRight = 0f;
        }

        float timePassed = 0;

        while (timePassed < animationTime) {
            foreach (RectTransform rec in transList) {
                MeshRenderer[] res = rec.gameObject.transform.GetComponentsInChildren<MeshRenderer>();

                float top = rec.offsetMax.y;
                float bottom = rec.offsetMin.y;

                float left = Mathf.Lerp(rec.offsetMin.x, endLeft, 0.08f);
                float right = Mathf.Lerp(rec.offsetMax.x, endRight, 0.08f);

                float deltaleft = left - rec.offsetMin.x;

                foreach (MeshRenderer m in res) {
                    float newX = m.gameObject.transform.localPosition.x + deltaleft;
                    float newY = m.gameObject.transform.localPosition.y;
                    float newZ = m.gameObject.transform.localPosition.z;
                    m.gameObject.transform.localPosition = new Vector3(newX, newY, newZ);
                }

                rec.offsetMin = new Vector2(left, bottom);
                rec.offsetMax = new Vector2(right, top);
            }

            timePassed += Time.deltaTime;
            yield return null;
        }

        foreach (RectTransform rec in transList) {
            float top = rec.offsetMax.y;
            float bottom = rec.offsetMin.y;

            float left = endLeft;
            float right = endRight;

            rec.offsetMin = new Vector2(left, bottom);
            rec.offsetMax = new Vector2(right, top);
        }

        yield return null;
    }

    private Color DarkenBarColor(List<Image> currList, Color theColor, bool isOn) {
        Color preserved = new Color(0, 0, 0);

        if (!lineCreated) {
            return preserved;
        }

        //print("Darken the Bar..");
        foreach (Image img in currList) {
            preserved = img.color;
            img.color = theColor;
        }

        return preserved;
    }

    private void SetBars(List<Image> currList, bool isOn) {
        if (!lineCreated) {
            return;
        }

        foreach (Image img in currList) {
            img.enabled = isOn;
        }
    }
    #endregion
}
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

    public static YearPanelManager Instance;
    public Color darkBlue;
    public Color darkYellow;
    public Color darkRed;

    //[HideInInspector]
    public GameObject parent = null;
    //[HideInInspector]
    public GameObject year0panel = null;
    //[HideInInspector]
    public GameObject year1panel = null;
    //[HideInInspector]
    public GameObject year2panel = null;
    //[HideInInspector]
    public GameObject year3panel = null;
    //[HideInInspector]
    public GameObject year4panel = null;
    //[HideInInspector]
    public GameObject theLineEditor = null;

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

    private bool lineCreated = false;
    private bool isPanelRetrieved = false;

    private bool isCooling = false;
    private bool isBackgroundOn = true;


    // Construct Line: 
    // Hide Background
    // Sapare items
    // dim bar color
    // hide bar color.
    // switch to family 

    public void setBackgrounds(bool isOn){

        if (!lineCreated) { return ; }

        foreach (Image img in allpanelBackgrounds){
            img.enabled = isOn;
        }

    }

    public void SeparatePanels(){

        if (isCooling) { return; }
        StartCoroutine(Cooling());

        //print("Separating panels...");
        movePanels(overallPanels, false);
        movePanels(bmiPanels, false);
        movePanels(ldlPanels, false);
        movePanels(bodyFatPanels, true);
        movePanels(hba1cPanels, true);
        movePanels(bpPanels, true);
    }

    public void DimAllBars(){
        darkenBarColor(normalBars, darkBlue, false);
        darkenBarColor(warningBars, darkYellow, false);
        darkenBarColor(upperBars, darkRed, false);
    }

    public void setAllBars(bool isOn){
        setBars(normalBars,isOn);
        setBars(warningBars, isOn);
        setBars(upperBars, isOn);
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        lineCreated = false;
    }

    public void GoAndRequestPanelInfo(){

        DebugText.Instance.Log("YearPanelManager get called");

        GameObject selected_human = HumanManager.Instance.getSelectedHuman();

        parent = selected_human.transform.Search("YearPanels").gameObject;
        year0panel = parent.transform.Search("CurrentYearPanel").gameObject;
        year1panel = parent.transform.Search("OneYearPanel").gameObject;
        year2panel = parent.transform.Search("TwoYearsPanel").gameObject;
        year3panel = parent.transform.Search("ThreeYearsPanel").gameObject;
        year4panel = parent.transform.Search("FourYearsPanel").gameObject;

        theLineEditor = parent.transform.Search("QuadLines").gameObject;

        DebugText.Instance.Log("Found selected human: " + selected_human);

        AllPanels.Add(year0panel);
        AllPanels.Add(year1panel);
        AllPanels.Add(year2panel);
        AllPanels.Add(year3panel);
        AllPanels.Add(year4panel);

        isPanelRetrieved = true;

        parseAllPanelsAndBars();

    }

    public void SetYearPanel(bool isOn)
    {
        if (MasterManager.Instance.GetCurrentGamePhase() != MasterManager.GamePhase.Phase5){
            DebugText.Instance.Log("Cannot maniplate year panel if not in Phase5");
            return;
        }

        // this should trigger the animations on the panels
        parent.SetActive(isOn);

    }

    public void ConstructYearPanelLines(){

        if (lineCreated){
            theLineEditor.SetActive(true);
            return;
        }

        if (MasterManager.Instance.GetCurrentGamePhase() != MasterManager.GamePhase.Phase5)
        {
            DebugText.Instance.Log("Cannot maniplate year panel if not in Phase5");
            return;
        }

        lineCreated = true;

        theLineEditor.GetComponent<QuadLine>().CreateAllLines();
    }

    public void HideLines(){
        if (lineCreated)
        {
            theLineEditor.SetActive(false);
        }
    }

    public void ToggleBioMetrics(string name){

        foreach (GameObject panel in AllPanels){

            GameObject target = panel.transform.Search(name).gameObject;
            target.SetActive(!target.activeSelf);

        }

    }



    private void parseAllPanelsAndBars()
    {

        if (!isPanelRetrieved) { return ; }

        foreach (GameObject yearpanel in AllPanels)
        {

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


    IEnumerator Cooling()
    {

        isCooling = true;

        yield return new WaitForSeconds(3f);

        isCooling = false;

        yield return null;
    }


    private void movePanels(List<GameObject> currPanels, bool isLeft)
    {
        if (!lineCreated) { return; }

        List<RectTransform> transList = new List<RectTransform>();

        foreach (GameObject panel in currPanels)
        {
            transList.Add(panel.GetComponent<RectTransform>());
        }

        StartCoroutine(MovePanelHelper(transList, isLeft));
    }

    IEnumerator MovePanelHelper(List<RectTransform> transList, bool isLeft)
    {

        float endLeft, endRight;

        float animation_time = 2.0f;

        if (isLeft)
        {
            endLeft = -400f;
            endRight = 400f;
        }
        else
        {
            endLeft = 400f;
            endRight = -400f;
        }

        float time_passed = 0;

        while (time_passed < animation_time)
        {

            foreach (RectTransform rec in transList)
            {
                MeshRenderer[] res = rec.gameObject.transform.GetComponentsInChildren<MeshRenderer>();


                float top = rec.offsetMax.y;
                float bottom = rec.offsetMin.y;

                float left = Mathf.Lerp(rec.offsetMin.x, endLeft, 0.08f);
                float right = Mathf.Lerp(rec.offsetMax.x, endRight, 0.08f);

                float deltaleft = left - rec.offsetMin.x;

                foreach (MeshRenderer m in res)
                {
                    float newX = m.gameObject.transform.localPosition.x + deltaleft;
                    float newY = m.gameObject.transform.localPosition.y;
                    float newZ = m.gameObject.transform.localPosition.z;
                    m.gameObject.transform.localPosition = new Vector3(newX, newY, newZ);
                }

                rec.offsetMin = new Vector2(left, bottom);
                rec.offsetMax = new Vector2(right, top);
            }

            time_passed += Time.deltaTime;
            yield return null;
        }

        foreach (RectTransform rec in transList)
        {
            float top = rec.offsetMax.y;
            float bottom = rec.offsetMin.y;

            float left = endLeft;
            float right = endRight;

            rec.offsetMin = new Vector2(left, bottom);
            rec.offsetMax = new Vector2(right, top);
        }

        yield return null;
    }

    private void darkenBarColor(List<Image> currList, Color theColor, bool isOn)
    {
        if (!lineCreated) { return; }

        //print("Darken the Bar..");
        foreach (Image img in currList)
        {
            img.color = theColor;
        }
    }

    private void setBars(List<Image> currList, bool isOn)
    {

        if (!lineCreated) { return; }

        foreach (Image img in currList)
        {
            img.enabled = isOn;
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)){
            AllPanels.Add(year0panel);
            AllPanels.Add(year1panel);
            AllPanels.Add(year2panel);
            AllPanels.Add(year3panel);
            AllPanels.Add(year4panel);

            isPanelRetrieved = true;
            lineCreated = true;

            parseAllPanelsAndBars();

            SeparatePanels();
        }

        if (Input.GetKeyDown(KeyCode.D)){
            setBackgrounds(!isBackgroundOn);
        }

        if (Input.GetKeyDown(KeyCode.F)){
            DimAllBars();
        }

        if (Input.GetKeyDown(KeyCode.G)){
            setAllBars(false);
        }

        if (Input.GetKeyDown(KeyCode.H)){
            setBackgrounds(false);
        }
    }
}

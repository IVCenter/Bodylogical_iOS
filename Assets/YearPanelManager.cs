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

    public static YearPanelManager Instance;

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

    private bool lineCreated;

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
}

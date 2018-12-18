using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Collections.Hybrid.Generic;

public class HumanManager : MonoBehaviour
{

    public static HumanManager Instance;
    public bool startSelectHuman;

    public GameObject male_prefab;
    public GameObject female_prefab;

    //public GameObject humanModel;

    private GameObject selected_human;
    private bool isAHumanSelected;
    private float cooling_time;

    private GameObject curr_resultPanel;

    public LinkedListDictionary<string, Archetype> archetypeMap;

    private bool yearPanelShowed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private bool CheckHumanSelection()
    {

        // DebugText.Instance.Log("Checking Human Selection...");
        foreach (Archetype human in archetypeMap.Values)
        {
            if (human.GetHumanObject().GetComponentInChildren<HumanInteract>().isSelected)
            {
                selected_human = human.GetHumanObject();
                return true;
            }
        }

        return false;
    }

    public bool MoveSelectedHumanToCenter()
    {

        if (!isAHumanSelected)
        {
            return false;
        }

        StartCoroutine(MoveHumanTowardCenter());

        return true;
    }

    public bool CreateArchitype(string ProfileName, string Name = "Bob", float Weight = 160, string sex = "male", string health_cond = "Good")
    {

        Archetype go = new Archetype(ProfileName, Name, Weight, sex, health_cond);
        go.InstantiateModel(male_prefab, female_prefab);

        if (!go.CreateModel(sex))
        {
            return false;
        }

        archetypeMap.Add(ProfileName, go);
        return true;
    }

    public bool IsAHumanSelected()
    {
        return isAHumanSelected;
    }


    public void IfExpandSelectedHumanInfo(bool expand)
    {
        if (selected_human == null)
        {
            return;
        }

        selected_human.transform.Search("BasicInfoCanvas").gameObject.SetActive(!expand);
        selected_human.transform.Search("DetailPanels").gameObject.SetActive(expand);
        cooling_time = 0f;
    }

    public void SetHumanCurrentYear(int year){
        selected_human.transform.Search("YearText").GetComponent<Text>().text = "Current Year: " + year;
        selected_human.transform.Search("BMIText").GetComponent<Text>().text = "" + (int)Random.Range(23, 42);
        selected_human.transform.Search("BodyFatText").GetComponent<Text>().text = "" + (int)Random.Range(23, 42);
        selected_human.transform.Search("CalorieText").GetComponent<Text>().text = "" + (int)Random.Range(1800, 3500);
        selected_human.transform.Search("SleepText").GetComponent<Text>().text = "" + (int)Random.Range(5,12);
        selected_human.transform.Search("BloodText").GetComponent<Text>().text = "" + (int)Random.Range(120, 150) + " / " + (int)Random.Range(50, 100);
    }

    public void ResetManager(){

        isAHumanSelected = false;
        selected_human = null;
        foreach (Archetype human in archetypeMap.Values){
            human.Clear();
        }
        archetypeMap.Clear();
    }

    public void HideUnselectedHuman(){
        foreach (Archetype human in archetypeMap.Values){
            if(human.GetHumanObject() != selected_human){
                human.GetHumanObject().SetActive(false);
            }
        }
    }


    public void FireChoicesNextPeriod(){

        IfExpandSelectedHumanInfo(false);
        SetHumanCurrentYear(2019);

        selected_human.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        selected_human.transform.Search("ChoicePanel").gameObject.SetActive(true);

        ButtonSequenceManager.Instance.SetPredictButton(false);
        TutorialText.Instance.Show("Select any path you want. For this demo, it doesn't matter.", 8.0f);
    }


    public void FireNextPeriod(int choice)
    {
        if (yearPanelShowed){
            return;
        }

        StartCoroutine(EnableYearPanels(choice));
        ButtonSequenceManager.Instance.SetLineChartButton(true);
        TutorialText.Instance.Show("Please select \"Line Chart\" to Create Ribbon Charts.", 12.0f);
    }

    public void CreateLineChart(){

        if(!yearPanelShowed){
            return;
        }

        YearPanelManager.Instance.ConstructYearPanelLines();
        ButtonSequenceManager.Instance.SetToggleButtons(true);
        ButtonSequenceManager.Instance.SetLineChartButton(false);

        TutorialText.Instance.ShowDouble("Nice! Now we are free to explore different interactions.", "Let's try toggle biometrics items and see how chart changes", 4.8f);

    }

    public IEnumerator EnableYearPanels(int choice){

        float animation_time = 1;

        selected_human.transform.Search("ChoicePanel").gameObject.SetActive(false);

        yield return ShiftHuman(-1, animation_time);

        // EnableOldStyleYearPanel(choice);


        YearPanelManager.Instance.SetYearPanel(true);

        yield return new WaitForSeconds(2f);

        yearPanelShowed = true;

        yield return null;
    }

    public void ResetPeriod(){

        if (!yearPanelShowed){
            return;
        }

        StartCoroutine(ShiftHuman(1, 0.2f));

        //curr_resultPanel.SetActive(false);
        //selected_human.transform.Search("ResultPanel").gameObject.SetActive(false);

        YearPanelManager.Instance.HideLines();
        YearPanelManager.Instance.SetYearPanel(false);
        selected_human.transform.Search("ChoicePanel").gameObject.SetActive(true);

        yearPanelShowed = false;

    }

    public GameObject getSelectedHuman(){
        return selected_human;
    }

    IEnumerator ShiftHuman(int amount, float animation_time){
        GameObject targetHuman = selected_human.transform.Search("male_model").gameObject;

        Vector3 des = new Vector3(targetHuman.transform.localPosition.x + amount, targetHuman.transform.localPosition.y, targetHuman.transform.localPosition.z);
        Vector3 init = targetHuman.transform.localPosition;

        float time_count = 0;
        while (time_count < animation_time){
            targetHuman.transform.localPosition = Vector3.Lerp(init, des, time_count/animation_time);
            time_count += Time.deltaTime;
            yield return null;
        }

        targetHuman.transform.localPosition = des;

        yield return null;
    }
   

    // Use this for initialization
    void Start () {
        archetypeMap = new LinkedListDictionary<string, Archetype>();
        isAHumanSelected = false;
        yearPanelShowed = false;
        cooling_time = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if(!isAHumanSelected){
            if(startSelectHuman && CheckHumanSelection()){
                isAHumanSelected = true;
                startSelectHuman = false;
            }
        }

        if(cooling_time < 3){
            cooling_time += Time.deltaTime;
        }
	}

    IEnumerator MoveHumanTowardCenter(){

        if (isAHumanSelected && selected_human != null)
        {
            float moveddist = 0;

            Vector3 startpos = selected_human.transform.position;
            Vector3 endpos = StageManager.Instance.stage.transform.position;

            float journey_length = Vector3.Distance(startpos, endpos);

            while (moveddist < journey_length)
            {
                float fracJourney = moveddist / journey_length;
                selected_human.transform.position = Vector3.Lerp(startpos, endpos, fracJourney);
                moveddist += Time.deltaTime;
                yield return null;
            }

            selected_human.transform.position = endpos;
            selected_human.transform.GetChild(1).transform.rotation = StageManager.Instance.stage.transform.rotation;
            selected_human.transform.GetChild(1).transform.Rotate(0, 180, 0);
        }

        yield return null;
    }


    private void EnableOldStyleYearPanel(int choice){

        selected_human.transform.Search("ResultPanel").gameObject.SetActive(true);

        if (choice == 0)
        {
            curr_resultPanel = selected_human.transform.Search("NothingPanel").gameObject;
        }

        if (choice == 1)
        {
            curr_resultPanel = selected_human.transform.Search("MinimumPanel").gameObject;
        }

        if (choice == 2)
        {
            curr_resultPanel = selected_human.transform.Search("RecommendedPanel").gameObject;
        }

        curr_resultPanel.SetActive(true);
    }

    
}

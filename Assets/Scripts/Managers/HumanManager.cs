using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Collections.Hybrid.Generic;

public class HumanManager : MonoBehaviour {

    public static HumanManager Instance;

    public bool startSelectHuman;

    public GameObject male_prefab;
    public GameObject female_prefab;

    public GameObject SelectedHuman {
        get; private set;
    }
    public bool IsHumanSelected {
        get; private set;
    }

    private float cooling_time;

    private GameObject curr_resultPanel;

    public LinkedListDictionary<string, Archetype> archetypeMap;

    private bool yearPanelShowed;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start() {
        archetypeMap = new LinkedListDictionary<string, Archetype>();
        IsHumanSelected = false;
        yearPanelShowed = false;
        cooling_time = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!IsHumanSelected) {
            if (startSelectHuman && CheckHumanSelection()) {
                IsHumanSelected = true;
                startSelectHuman = false;
            }
        }

        if (cooling_time < 3) {
            cooling_time += Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks if a human model is selected.
    /// </summary>
    /// <returns><c>true</c>, if a model is selected, <c>false</c> otherwise.</returns>
    private bool CheckHumanSelection() {
        // DebugText.Instance.Log("Checking Human Selection...");
        foreach (Archetype human in archetypeMap.Values) {
            if (human.HumanObject.GetComponentInChildren<HumanInteract>().isSelected) {
                SelectedHuman = human.HumanObject;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Starts a coroutine to move the selected model to center of stage.
    /// </summary>
    public bool MoveSelectedHumanToCenter() {
        if (!IsHumanSelected) {
            return false;
        }

        StartCoroutine(MoveHumanTowardCenter());
        return true;
    }

    /// <summary>
    /// Moves the human toward center.
    /// </summary>
    IEnumerator MoveHumanTowardCenter() {
        if (IsHumanSelected && SelectedHuman != null) {
            float movedDist = 0;

            Vector3 startpos = SelectedHuman.transform.position;
            Vector3 endpos = StageManager.Instance.stage.transform.position;

            float journey_length = Vector3.Distance(startpos, endpos);

            while (movedDist < journey_length) {
                float fracJourney = movedDist / journey_length;
                SelectedHuman.transform.position = Vector3.Lerp(startpos, endpos, fracJourney);
                movedDist += Time.deltaTime;
                yield return null;
            }

            SelectedHuman.transform.position = endpos;
            SelectedHuman.transform.GetChild(1).transform.rotation = StageManager.Instance.stage.transform.rotation;
        }

        yield return null;
    }

    /// <summary>
    /// When one model is selected, hide others.
    /// </summary>
    public void HideUnselectedHuman() {
        foreach (Archetype human in archetypeMap.Values) {
            if (human.HumanObject != SelectedHuman) {
                human.HumanObject.SetActive(false);
            }
        }
    }

    public void IfExpandSelectedHumanInfo(bool expand) {
        if (SelectedHuman == null) {
            return;
        }

        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(!expand);
        SelectedHuman.transform.Search("DetailPanels").gameObject.SetActive(expand);
        cooling_time = 0f;
    }


    public bool CreateArchitype(string ProfileName, string Name = "Bob", float Weight = 160, string sex = "male", string health_cond = "Good") {
        Archetype go = new Archetype(ProfileName, Name, Weight, sex, health_cond);
        go.InstantiateModel(male_prefab, female_prefab);

        if (!go.CreateModel(sex)) {
            return false;
        }

        archetypeMap.Add(ProfileName, go);
        return true;
    }

    public void SetHumanCurrentYear(int year) {
        SelectedHuman.transform.Search("YearText").GetComponent<Text>().text = "Current Year: " + year;
        SelectedHuman.transform.Search("BMIText").GetComponent<Text>().text = "" + (int)Random.Range(23, 42);
        SelectedHuman.transform.Search("BodyFatText").GetComponent<Text>().text = "" + (int)Random.Range(23, 42);
        SelectedHuman.transform.Search("CalorieText").GetComponent<Text>().text = "" + (int)Random.Range(1800, 3500);
        SelectedHuman.transform.Search("SleepText").GetComponent<Text>().text = "" + (int)Random.Range(5, 12);
        SelectedHuman.transform.Search("BloodText").GetComponent<Text>().text = "" + (int)Random.Range(120, 150) + " / " + (int)Random.Range(50, 100);
    }

    public void ResetManager() {
        IsHumanSelected = false;
        SelectedHuman = null;
        foreach (Archetype human in archetypeMap.Values) {
            human.Clear();
        }
        archetypeMap.Clear();
    }




    public void FireChoicesNextPeriod() {
        IfExpandSelectedHumanInfo(false);
        SetHumanCurrentYear(2019);

        SelectedHuman.transform.Search("BasicInfoCanvas").gameObject.SetActive(false);
        SelectedHuman.transform.Search("ChoicePanel").gameObject.SetActive(true);

        ButtonSequenceManager.Instance.SetPredictButton(false);
        TutorialText.Instance.Show("Select any path you want. For this demo, it doesn't matter.", 8.0f);
    }


    public void FireNextPeriod(int choice) {
        if (yearPanelShowed) {
            return;
        }

        StartCoroutine(EnableYearPanels(choice));
        ButtonSequenceManager.Instance.SetLineChartButton(true);
        TutorialText.Instance.Show("Please select \"Line Chart\" to Create Ribbon Charts.", 12.0f);
    }

    public void CreateLineChart() {
        if (!yearPanelShowed) {
            return;
        }

        YearPanelManager.Instance.ConstructYearPanelLines();
        ButtonSequenceManager.Instance.SetToggleButtons(true);
        ButtonSequenceManager.Instance.SetLineChartButton(false);

        TutorialText.Instance.ShowDouble("Nice! Now we are free to explore different interactions.", "Let's try toggle biometrics items and see how chart changes", 4.8f);

    }

    public IEnumerator EnableYearPanels(int choice) {
        float animation_time = 1;

        SelectedHuman.transform.Search("ChoicePanel").gameObject.SetActive(false);

        yield return ShiftHuman(-1, animation_time);

        // EnableOldStyleYearPanel(choice);


        YearPanelManager.Instance.SetYearPanel(true);

        yield return new WaitForSeconds(2f);

        yearPanelShowed = true;

        yield return null;
    }

    public void ResetPeriod() {
        if (!yearPanelShowed) {
            return;
        }

        StartCoroutine(ShiftHuman(1, 0.2f));

        //curr_resultPanel.SetActive(false);
        //SelectedHuman.transform.Search("ResultPanel").gameObject.SetActive(false);

        YearPanelManager.Instance.HideLines();
        YearPanelManager.Instance.SetYearPanel(false);
        SelectedHuman.transform.Search("ChoicePanel").gameObject.SetActive(true);

        yearPanelShowed = false;

    }

    IEnumerator ShiftHuman(int amount, float animation_time) {
        GameObject targetHuman = SelectedHuman.transform.Search("ModelParent").gameObject;

        Vector3 des = new Vector3(targetHuman.transform.localPosition.x + amount, targetHuman.transform.localPosition.y, targetHuman.transform.localPosition.z);
        Vector3 init = targetHuman.transform.localPosition;

        float time_count = 0;
        while (time_count < animation_time) {
            targetHuman.transform.localPosition = Vector3.Lerp(init, des, time_count / animation_time);
            time_count += Time.deltaTime;
            yield return null;
        }

        targetHuman.transform.localPosition = des;

        yield return null;
    }


    private void EnableOldStyleYearPanel(int choice) {
        SelectedHuman.transform.Search("ResultPanel").gameObject.SetActive(true);

        if (choice == 0) {
            curr_resultPanel = SelectedHuman.transform.Search("NothingPanel").gameObject;
        }

        if (choice == 1) {
            curr_resultPanel = SelectedHuman.transform.Search("MinimumPanel").gameObject;
        }

        if (choice == 2) {
            curr_resultPanel = SelectedHuman.transform.Search("RecommendedPanel").gameObject;
        }

        curr_resultPanel.SetActive(true);
    }
}

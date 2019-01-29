using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberPanelManager : MonoBehaviour {
    public static NumberPanelManager Instance { get; private set; }

    public GameObject detailPanels;
    public GameObject choicePanels;
    
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void UpdateDetailPanels() {
        detailPanels.transform.Search("YearText").GetComponent<Text>().text = "Current Year: 2019";
        detailPanels.transform.Search("BMIText").GetComponent<Text>().text = "" + (int)Random.Range(23, 42);
        detailPanels.transform.Search("BodyFatText").GetComponent<Text>().text = "" + (int)Random.Range(23, 42);
        detailPanels.transform.Search("CalorieText").GetComponent<Text>().text = "" + (int)Random.Range(1800, 3500);
        detailPanels.transform.Search("SleepText").GetComponent<Text>().text = "" + (int)Random.Range(5, 12);
        detailPanels.transform.Search("BloodText").GetComponent<Text>().text = "" + (int)Random.Range(120, 150) + " / " + (int)Random.Range(50, 100);
    }

    public void ToggleDetailPanels(bool on) {
        detailPanels.SetActive(on);
    }

    public void ToggleChoicePanels(bool on) {
        choicePanels.SetActive(on);
    }
}

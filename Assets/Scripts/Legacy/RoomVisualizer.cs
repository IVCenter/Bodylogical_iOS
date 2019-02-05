using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomVisualizer : MonoBehaviour {
    public GameObject room;
    public Text headerText;
    [Header("Please arrange in order of Bad, Intermediate, and Good")]
    public Material[] roomIndicatorMaterials;
    public GameObject[] humanModels;
    [Header("Limited to one category only")]
    public GameObject[] company;
    public GameObject[] independence;
    public GameObject[] badActivities, intermediateActivities, goodActivities;

    void Awake() {
        HideAll();
    }

    public void UpdateHeader(int year, string path) {
        headerText.text = year + " years Later (" + path + ")";
    }

    /// <summary>
    /// Show/Hide appropriate props based on health point.
    /// </summary>
    /// <param name="point">A point given in 1-10 scale. 1 means very bad and 10 means very good.</param>
    public void Visualize(int point) {
        HideAll(); // hide all then activate as needed

        ChangeRoomMaterial(point);

        if (point <= 1) { // extremely bad! Show everything bad!
            humanModels[0].SetActive(true);
            foreach (GameObject activity in badActivities) {
                activity.SetActive(true);
            }
            foreach (GameObject indep in independence) {
                indep.SetActive(true);
            }

        } else if (point <= 3) { // not so bad. Show something bad.
            humanModels[1].SetActive(true);
            for (int i = 0; i < independence.Length / 2; i++) {
                independence[i].SetActive(true);
            }

            for (int i = 0; i < intermediateActivities.Length / 3; i++) {
                intermediateActivities[i].SetActive(true);
            }

        } else if (point <= 5) { // intermediate. Show few bad, but some intermediary.
            humanModels[1].SetActive(true);
            for (int i = 0; i < independence.Length / 3; i++) {
                independence[i].SetActive(true);
            }

            for (int i = 0; i < intermediateActivities.Length / 2; i++) {
                intermediateActivities[i].SetActive(true);
            }
        } else if (point <= 7) { // doing somewhat well. No bad, many intermediary.
            humanModels[1].SetActive(true);
            for (int i = 0; i < intermediateActivities.Length; i++) {
                intermediateActivities[i].SetActive(true);
            }
            for (int i = 0; i < company.Length / 2; i++) {
                company[i].SetActive(true);
            }

        } else if (point <= 9) { // very well. Show few intermediary, and some good.
            humanModels[1].SetActive(true);
            for (int i = 0; i < intermediateActivities.Length / 3; i++) {
                intermediateActivities[i].SetActive(true);
            }
            for (int i = 0; i < goodActivities.Length / 2; i++) {
                goodActivities[i].SetActive(true);
            }
            for (int i = 0; i < company.Length; i++) {
                company[i].SetActive(true);
            }

        } else { // perfect health! Show no intermediary and all good.
            humanModels[1].SetActive(true);
            for (int i = 0; i < goodActivities.Length; i++) {
                goodActivities[i].SetActive(true);
            }
            for (int i = 0; i < company.Length; i++) {
                company[i].SetActive(true);
            }
        }
    }

    public void HideAll() {
        foreach (GameObject activity in badActivities) {
            activity.SetActive(false);
        }

        foreach (GameObject activity in intermediateActivities) {
            activity.SetActive(false);
        }

        foreach (GameObject activity in goodActivities) {
            activity.SetActive(false);
        }

        foreach (GameObject comp in company) {
            comp.SetActive(false);
        }

        foreach (GameObject indep in independence) {
            indep.SetActive(false);
        }

        foreach (GameObject humanModel in humanModels) {
            humanModel.SetActive(false);
        }
    }

    /// <summary>
    /// Changes the room materialbased on health point.
    /// </summary>
    /// <param name="point">A point given in 1-10 scale. 1 means very bad and 10 means very good.</param>
    public void ChangeRoomMaterial(int point) {
        if (point <= 4) {
            room.GetComponent<MeshRenderer>().material = roomIndicatorMaterials[0];
        } else if (point <= 7) {
            room.GetComponent<MeshRenderer>().material = roomIndicatorMaterials[1];
        } else {
            room.GetComponent<MeshRenderer>().material = roomIndicatorMaterials[2];
        }
    }
}

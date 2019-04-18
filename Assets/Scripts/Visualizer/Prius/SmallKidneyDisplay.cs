using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallKidneyDisplay : OrganDisplay {
    public GameObject goodLeftKidney, badLeftKidney, goodRightKidney, badRightKidney;
    private bool LeftSelected { get { return PriusManager.Instance.KidneyLeft; } }
    private GameObject CurrGood { get { return LeftSelected ? goodLeftKidney : goodRightKidney; } }
    private GameObject OtherGood { get { return LeftSelected ? goodRightKidney : goodLeftKidney; } }
    private GameObject CurrBad { get { return LeftSelected ? badLeftKidney : badRightKidney; } }
    private GameObject OtherBad { get { return LeftSelected ? badRightKidney : badLeftKidney; } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            // only one kidney is shown (other, since curr is enlarged)
            if (PriusManager.Instance.currentPart == PriusType.Kidney) {
                if (status == HealthStatus.Good) {
                    OtherGood.SetActive(true);
                    OtherBad.SetActive(false);
                } else {
                    OtherGood.SetActive(false);
                    OtherBad.SetActive(true);
                }
                CurrGood.SetActive(false);
                CurrBad.SetActive(false);
            } else { // other kidney is shown
                if (status == HealthStatus.Good) {
                    CurrGood.SetActive(true);
                    OtherGood.SetActive(true);
                    CurrBad.SetActive(false);
                    OtherBad.SetActive(false);
                } else {
                    CurrGood.SetActive(false);
                    OtherGood.SetActive(false);
                    CurrBad.SetActive(true);
                    OtherBad.SetActive(true);
                }
            }

        }
    }
}

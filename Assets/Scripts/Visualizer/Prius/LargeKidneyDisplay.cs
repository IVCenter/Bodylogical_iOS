using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeKidneyDisplay : OrganDisplay {
    public GameObject leftKidney, rightKidney;
    private bool LeftSelected { get { return PriusManager.Instance.KidneyLeft; } }
    private GameObject Curr { get { return LeftSelected ? leftKidney : rightKidney; } }
    private GameObject Other { get { return LeftSelected ? rightKidney : leftKidney; } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            Curr.SetActive(true);
            Other.SetActive(false);
        }
    }
}

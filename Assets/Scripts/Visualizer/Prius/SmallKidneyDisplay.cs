using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallKidneyDisplay : OrganDisplay {
    public GameObject leftKidney, rightKidney;

    private bool LeftSelected { get { return PriusManager.Instance.KidneyLeft; } }

    private GameObject CurrKidney { get { return LeftSelected ? leftKidney : rightKidney; } }
    private GameObject OtherKidney { get { return LeftSelected ? rightKidney : leftKidney; } }

    private SkinnedMeshRenderer CurrRenderer { get { return CurrKidney.GetComponent<SkinnedMeshRenderer>(); } }
    private SkinnedMeshRenderer OtherRenderer { get { return OtherKidney.GetComponent<SkinnedMeshRenderer>(); } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            // Only OtherKidney is shown; CurrKidney is enlarged and will
            // be managed by LargeKidneyDisplay, so it will be hidden.
            if (PriusManager.Instance.currentPart == PriusType.Kidney) {
                CurrKidney.SetActive(false);
                OtherKidney.SetActive(true);
                // Blend shape for the kidney is from "good" to "bad".
                // 0 means good, and 100 means bad.
                // So we need to reverse the score.
                OtherRenderer.SetBlendShapeWeight(0, 100 - score);
            } else {
                // Both kidneys will be shown.
                CurrKidney.SetActive(true);
                OtherKidney.SetActive(true);
                CurrRenderer.SetBlendShapeWeight(0, 100 - score);
                OtherRenderer.SetBlendShapeWeight(0, 100 - score);
            }

        }
    }
}

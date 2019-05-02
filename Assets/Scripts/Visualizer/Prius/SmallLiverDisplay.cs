using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLiverDisplay : OrganDisplay {
    public GameObject liver;

    public SkinnedMeshRenderer LiverRenderer { get { return liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>(); } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            LiverRenderer.SetBlendShapeWeight(0, 100 - score);
        }
    }
}

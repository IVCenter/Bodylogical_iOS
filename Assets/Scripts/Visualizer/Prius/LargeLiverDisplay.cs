using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeLiverDisplay : OrganDisplay {
    public GameObject liver;

    public SkinnedMeshRenderer LiverRenderer { get { return liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>(); } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    LiverRenderer.SetBlendShapeWeight(0, 100 - score);
                    break;
                case PriusShowStatus.Bad:
                    LiverRenderer.SetBlendShapeWeight(0, 100);
                    break;
                case PriusShowStatus.Intermediate:
                    LiverRenderer.SetBlendShapeWeight(0, 50);
                    break;
                case PriusShowStatus.Good:
                    LiverRenderer.SetBlendShapeWeight(0, 0);
                    break;
            }
        }
    }
}

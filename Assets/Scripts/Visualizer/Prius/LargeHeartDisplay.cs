using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeHeartDisplay : OrganDisplay {
    public GameObject heart;
    private Animator HeartAnimator { get { return heart.transform.GetChild(0).GetComponent<Animator>(); } }
    private SkinnedMeshRenderer VesselRenderer { get { return heart.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>(); } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            heart.SetActive(true);
            // calculate animation speed
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    HeartAnimator.speed = 1.0f - score / 100.0f;
                    VesselRenderer.SetBlendShapeWeight(0, 100 - score);
                    break;
                case PriusShowStatus.Bad:
                    HeartAnimator.speed = 1.0f;
                    VesselRenderer.SetBlendShapeWeight(0, 100);
                    break;
                case PriusShowStatus.Intermediate:
                    HeartAnimator.speed = 0.75f;
                    VesselRenderer.SetBlendShapeWeight(0, 50);
                    break;
                case PriusShowStatus.Good:
                    HeartAnimator.speed = 0.5f;
                    VesselRenderer.SetBlendShapeWeight(0, 0);
                    break;
            }
        }
    }
}

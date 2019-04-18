using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDisplay : OrganDisplay {
    public GameObject heart;
    private Animator HeartAnimator { get { return heart.transform.GetChild(0).GetComponent<Animator>(); } }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            heart.SetActive(true);
            // calculate animation speed
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    HeartAnimator.speed = 1.0f - score / 100.0f;
                    break;
                case PriusShowStatus.Bad:
                    HeartAnimator.speed = 1.0f;
                    break;
                case PriusShowStatus.Intermediate:
                    HeartAnimator.speed = 0.75f;
                    break;
                case PriusShowStatus.Good:
                    HeartAnimator.speed = 0.5f;
                    break;
            }
        }
    }
}

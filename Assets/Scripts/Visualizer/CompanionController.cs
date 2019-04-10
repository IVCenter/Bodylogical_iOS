using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour {
    public Animator normalAnimator;
    public Animator weightedAnimator;

    public Animator CurrentAnimator {
        get {
            if (TimeProgressManager.Instance.Year < 15) {
                normalAnimator.gameObject.SetActive(true);
                weightedAnimator.gameObject.SetActive(false);
                return normalAnimator;
            } else {
                normalAnimator.gameObject.SetActive(false);
                weightedAnimator.gameObject.SetActive(true);
                return weightedAnimator;
            }
        }
    }
}

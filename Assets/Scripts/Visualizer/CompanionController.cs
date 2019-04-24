using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour {
    public Gender gender;
    public Animator normalAnimator;
    public Animator agedAnimator;

    public Animator CurrentAnimator {
        get {
            if (TimeProgressManager.Instance.YearCount < 15) {
                normalAnimator.gameObject.SetActive(true);
                agedAnimator.gameObject.SetActive(false);
                return normalAnimator;
            } else {
                normalAnimator.gameObject.SetActive(false);
                agedAnimator.gameObject.SetActive(true);
                return agedAnimator;
            }
        }
    }
}

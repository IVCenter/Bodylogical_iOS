using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairController : MonoBehaviour {
    public Transform pusherTransform;

    private Animator otherAnimator;
    private Vector3 otherPosition;
    private Quaternion otherRotation;

    public void ToggleOn() {
        gameObject.SetActive(true);
        transform.position = HumanManager.Instance.SelectedHuman.transform.position;
        transform.rotation = HumanManager.Instance.SelectedHuman.transform.rotation;

        ActivityManager.Instance.OtherCompanion.ToggleLegend(false);
        ActivityManager.Instance.OtherCompanion.gameObject.SetActive(true);
        otherPosition = ActivityManager.Instance.OtherTransform.position;
        ActivityManager.Instance.OtherTransform.position = pusherTransform.position;
        otherRotation = ActivityManager.Instance.OtherTransform.rotation;
        ActivityManager.Instance.OtherTransform.rotation = pusherTransform.rotation;

        // Since companion may change (from younger to older and vise versa),
        // need to check if the companion has changed.
        if (ActivityManager.Instance.OtherAnimator != otherAnimator) {
            otherAnimator = ActivityManager.Instance.OtherAnimator;
            otherAnimator.SetTrigger("PushWheelchair");
        }
    }


    public void ToggleOff() {
        gameObject.SetActive(false);
        ActivityManager.Instance.OtherCompanion.gameObject.SetActive(false);
        ActivityManager.Instance.OtherTransform.position = otherPosition;
        ActivityManager.Instance.OtherTransform.rotation = otherRotation;
        otherAnimator = null;
    }
}

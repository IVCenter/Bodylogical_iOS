using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A temporary script to move the human back to the original point.
/// This script is attached to the human model template as well as the kicking partner.
/// TODO: replace this script with an animation that wouldn't move the human model.
/// </summary>
public class MoveBack : MonoBehaviour {
    public Animator animator;
    public GameObject humanModel;
    public Vector3 moveToPoint;

    private bool kickingFinished;

    // Update is called once per frame
    void Update() {
        if (animator != null) {
            if (IsKicking()) {
                kickingFinished = false;
            } else if (!kickingFinished) {
                kickingFinished = true;
                humanModel.transform.localPosition = moveToPoint;
            }
        }
    }

    bool IsKicking() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Kick");
    }
}

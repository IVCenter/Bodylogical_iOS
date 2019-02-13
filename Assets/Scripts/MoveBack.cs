using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A temporary script to move the human back to the original point.
/// </summary>
public class MoveBack : MonoBehaviour {
    public Animator animator;
    public GameObject humanModel;

    private bool kickingFinished;

    // Update is called once per frame
    void Update() {
        if (IsKicking()) {
            kickingFinished = false;
        } else if (!kickingFinished) {
            kickingFinished = true;
            humanModel.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    bool IsKicking() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Kick");
    }
}

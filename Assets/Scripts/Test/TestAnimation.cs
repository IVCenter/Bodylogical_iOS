using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimation : MonoBehaviour {
    public Animator animator;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            animator.SetTrigger("IdlePose");
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            animator.SetTrigger("Idle");
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            animator.SetTrigger("Jog");
        }
    }
}

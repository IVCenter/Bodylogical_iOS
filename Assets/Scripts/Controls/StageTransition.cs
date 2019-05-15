using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTransition : MonoBehaviour {
    public Transform leftDoor, rightDoor;
    public float animationTime = 2.0f;

    IEnumerator OpenStage() {
        int timeSteps = (int)(animationTime / Time.deltaTime);
        float moveStep = 1.0f / timeSteps;

        // Shift localPosition.x. left: 0 -> -1, right: 0 -> 1
        // Translate takes two parameters; the second defaults to Space.Self.
        for (int i = 0; i < timeSteps; i++) {
            leftDoor.Translate(new Vector3(-moveStep, 0, 0));
            rightDoor.Translate(new Vector3(moveStep, 0, 0));
            yield return null;
        }
    }

    IEnumerator CloseStage() {
        int timeSteps = (int)(animationTime / Time.deltaTime);
        float moveStep = 1.0f / timeSteps;

        // Shift localPosition.x. left: -1 -> 0, right: 1 -> 0
        // Translate takes two parameters; the second defaults to Space.Self.
        for (int i = 0; i < timeSteps; i++) {
            leftDoor.Translate(new Vector3(moveStep, 0, 0));
            rightDoor.Translate(new Vector3(-moveStep, 0, 0));
            yield return null;
        }
    }
}

using System.Collections;
using UnityEngine;

public class SwitcherAnimation : ComponentAnimation {
    public Transform top, left, right;
    public float animationTime = 1.0f;
    public Switcher switcher;

    public override IEnumerator Animate() {
        Vector3 currPos = transform.localPosition;
        Vector3 nextPos;
        switch (switcher.currOption) {
            case 0: // top
                nextPos = top.localPosition;
                break;
            case 1: // left
                nextPos = left.localPosition;
                break;
            default: // right (2)
                nextPos = right.localPosition;
                break;
        }
        
        int steps = (int)(animationTime / Time.deltaTime);
        for (int i = 1; i <= steps; i++) {
            transform.localPosition = Vector3.Lerp(currPos, nextPos, (float)i / steps);
            yield return null;
        }

        anim = null;
    }
}

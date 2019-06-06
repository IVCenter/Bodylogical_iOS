using System.Collections;
using UnityEngine;

public class SwitcherAnimation : ComponentAnimation {
    public Transform[] options;
    public float animationTime = 1.0f;
    public Switcher switcher;

    public override IEnumerator Animate(System.Action callback) {
        Vector3 currPos = transform.localPosition;
        Vector3 nextPos = options[switcher.currOption].localPosition;
        
        int steps = (int)(animationTime / Time.deltaTime);
        for (int i = 1; i <= steps; i++) {
            transform.localPosition = Vector3.Lerp(currPos, nextPos, (float)i / steps);
            yield return null;
        }

        anim = null;
        if (callback != null) {
            callback();
        }
    }
}

using System.Collections;
using UnityEngine;

public class SwitcherAnimation : ComponentAnimation {
    public Transform[] options;
    public float animationTime = 0.5f;
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
        callback?.Invoke();
    }

    /// <summary>
    /// Simply jumps to the corresponding option.
    /// </summary>
    public override void Jump() {
        transform.localPosition = options[switcher.currOption].localPosition;
    }
}

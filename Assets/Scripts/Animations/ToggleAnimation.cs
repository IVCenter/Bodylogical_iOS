using System.Collections;
using UnityEngine;

public class ToggleAnimation : ComponentAnimation {
    public float animationTime = 0.5f;

    public override IEnumerator Animate(System.Action callback) {
        Vector3 currTransform = transform.localPosition;
        float destX = -currTransform.x;
        int steps = (int)(animationTime / Time.deltaTime);
        for (int i = 1; i <= steps; i++) {
            transform.localPosition = new Vector3(
                Mathf.Lerp(currTransform.x, destX, (float)i / steps),
                currTransform.y, currTransform.z);
            yield return null;
        }

        anim = null;
        if (callback != null) {
            callback();
        }
    }

    /// <summary>
    /// A toggle has two states. Just change to another state.
    /// </summary>
    public override void Jump() {
        transform.localPosition = new Vector3(-transform.localPosition.x,
            transform.localPosition.y,
            transform.localPosition.z);
    }
}

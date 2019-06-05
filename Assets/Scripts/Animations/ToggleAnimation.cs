using System.Collections;
using UnityEngine;

public class ToggleAnimation : ComponentAnimation {
    public float animationTime = 1.0f;

    public override IEnumerator Animate() {
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
    }
}

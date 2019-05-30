using System.Collections;
using UnityEngine;

/// <summary>
/// A button press animation.
/// </summary>
public class LargeButtonAnimation : ComponentAnimation {
    public Transform buttonBase;

    public Transform base1, base2;

    public float animationTime = 1.5f;
    /// <summary>
    /// The proportion of animationTime that is allocated for "press to base".
    /// </summary>
    [Range(0, 1)]
    public float toBaseRate = 0.7f;

    /// <summary>
    /// Original local position of the button.
    /// </summary>
    private Vector3 origPos, base1OrigPos, base2OrigPos;

    void Start() {
        origPos = transform.localPosition;
        base1OrigPos = base1.localPosition;
        base2OrigPos = base2.localPosition;
    }

    public override IEnumerator Animate() {
        // Because we need three loops, the step count is divided by 3.
        int steps = (int)(animationTime / Time.deltaTime) / 3;

        int baseSteps = (int)(steps * toBaseRate);

        // First pass: from original position to base
        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(origPos, base1OrigPos, 
                Mathf.Lerp(0, baseSteps, i));
            yield return null;
        }

        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(base1OrigPos, base2OrigPos,
                Mathf.Lerp(0, baseSteps, i));
            base1.localPosition = Vector3.Lerp(base1OrigPos, base2OrigPos,
                Mathf.Lerp(0, baseSteps, i));
            yield return null;
        }

        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(base2OrigPos, buttonBase.localPosition,
                Mathf.Lerp(0, baseSteps / 3, i));
            base1.localPosition = Vector3.Lerp(base2OrigPos, buttonBase.localPosition,
                Mathf.Lerp(0, baseSteps / 3, i));
            base2.localPosition = Vector3.Lerp(base2OrigPos, buttonBase.localPosition,
                Mathf.Lerp(0, baseSteps / 3, i));
            yield return null;
        }

        // Second pass: from base to original position
        for (int i = 0; i < steps - baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(buttonBase.localPosition, base2OrigPos,
                Mathf.Lerp(0, baseSteps / 3, i));
            base1.localPosition = Vector3.Lerp(buttonBase.localPosition, base2OrigPos,
                Mathf.Lerp(0, baseSteps / 3, i));
            base2.localPosition = Vector3.Lerp(buttonBase.localPosition, base2OrigPos,
                Mathf.Lerp(0, baseSteps / 3, i));
            yield return null;
        }

        for (int i = 0; i < steps - baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(base2OrigPos, base1OrigPos,
                Mathf.Lerp(0, baseSteps / 3, i));
            base1.localPosition = Vector3.Lerp(base2OrigPos, base1OrigPos,
                Mathf.Lerp(0, baseSteps / 3, i));
            yield return null;
        }

        for (int i = 0; i < steps - baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(base1OrigPos, origPos,
                Mathf.Lerp(0, baseSteps / 3, i));
            yield return null;
        }

        anim = null;
    }
}

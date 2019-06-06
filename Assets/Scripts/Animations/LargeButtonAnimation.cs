using System.Collections;
using UnityEngine;

/// <summary>
/// A button press animation.
/// </summary>
public class LargeButtonAnimation : ComponentAnimation {
    public Transform buttonBase;

    public Transform base1, base2;

    public float animationTime = 1.0f;
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

    public override IEnumerator Animate(System.Action callback) {
        // Because we need three loops, the step count is divided by 3.
        int steps = (int)(animationTime / Time.deltaTime) / 3;

        int baseSteps = (int)(steps * toBaseRate);
        int remainSteps = steps - baseSteps;

        // First pass: from original position to base
        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(origPos, base1OrigPos, 
                (float)i / baseSteps);
            yield return null;
        }

        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(base1OrigPos, base2OrigPos,
                (float)i / baseSteps);
            base1.localPosition = Vector3.Lerp(base1OrigPos, base2OrigPos,
                (float)i / baseSteps);
            yield return null;
        }

        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(base2OrigPos, buttonBase.localPosition,
                (float)i / baseSteps);
            base1.localPosition = Vector3.Lerp(base2OrigPos, buttonBase.localPosition,
                (float)i / baseSteps);
            base2.localPosition = Vector3.Lerp(base2OrigPos, buttonBase.localPosition,
                (float)i / baseSteps);
            yield return null;
        }

        // Second pass: from base to original position
        for (int i = 0; i < remainSteps; i++) {
            transform.localPosition = Vector3.Lerp(buttonBase.localPosition, base2OrigPos,
                (float)i / remainSteps);
            base1.localPosition = Vector3.Lerp(buttonBase.localPosition, base2OrigPos,
                (float)i / remainSteps);
            base2.localPosition = Vector3.Lerp(buttonBase.localPosition, base2OrigPos,
                (float)i / remainSteps);
            yield return null;
        }

        for (int i = 0; i < remainSteps; i++) {
            transform.localPosition = Vector3.Lerp(base2OrigPos, base1OrigPos,
                (float)i / remainSteps);
            base1.localPosition = Vector3.Lerp(base2OrigPos, base1OrigPos,
                (float)i / remainSteps);
            yield return null;
        }

        for (int i = 0; i < remainSteps; i++) {
            transform.localPosition = Vector3.Lerp(base1OrigPos, origPos,
                (float)i / remainSteps);
            yield return null;
        }

        anim = null;
        if(callback != null) {
            callback();
        }
    }

    /// <summary>
    /// The animation is simply button pushing, so after this nothing has changed.
    /// (the button has returned to non-pressed state)
    /// </summary>
    public override void Jump() { }
}

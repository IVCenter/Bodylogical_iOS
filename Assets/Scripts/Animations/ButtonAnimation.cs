using System.Collections;
using UnityEngine;

/// <summary>
/// A button press animation.
/// </summary>
public class ButtonAnimation : ComponentAnimation {
    public Transform buttonBase;

    public float animationTime = 1.5f;
    /// <summary>
    /// The proportion of animationTime that is allocated for "press to base".
    /// </summary>
    [Range(0, 1)]
    public float toBaseRate = 0.7f;

    /// <summary>
    /// Original local position of the button.
    /// </summary>
    private Vector3 origPos;

    void Start() {
        origPos = transform.localPosition;
    }

    public override IEnumerator Animate() {
        int steps = (int)(animationTime / Time.deltaTime);

        int baseSteps = (int)(steps * toBaseRate);

        // First pass: from original position to base
        for (int i = 0; i < baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(origPos, buttonBase.localPosition, 
                Mathf.Lerp(0, baseSteps, i));
            yield return null;
        }

        // Second pass: from base to original position
        for (int i = 0; i < steps - baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(buttonBase.localPosition, origPos,
                Mathf.Lerp(0, baseSteps, i));
            yield return null;
        }

        anim = null;
    }
}

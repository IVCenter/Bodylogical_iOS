using System.Collections;
using UnityEngine;

/// <summary>
/// A button press animation.
/// </summary>
public class ButtonAnimation : ComponentAnimation {
    public Transform buttonBase;

    public float animationTime = 0.5f;
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

    public override IEnumerator Animate(System.Action callback = null) {
        int steps = (int)(animationTime / Time.deltaTime);

        int baseSteps = (int)(steps * toBaseRate);

        // First pass: from original position to base
        for (int i = 1; i <= baseSteps; i++) {
            transform.localPosition = Vector3.Lerp(origPos, buttonBase.localPosition, 
                (float)i / baseSteps);
            yield return null;
        }

        int returnSteps = steps - baseSteps;
        // Second pass: from base to original position
        for (int i = 1; i <= returnSteps; i++) {
            transform.localPosition = Vector3.Lerp(buttonBase.localPosition, origPos,
                (float)i / returnSteps);
            yield return null;
        }

        anim = null;
        callback?.Invoke();
    }

    /// <summary>
    /// The animation is simply button pushing, so after this nothing has changed.
    /// (the button has returned to non-pressed state)
    /// </summary>
    public override void Jump() { }
}

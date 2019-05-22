using System.Collections;
using UnityEngine;

public class ButtonTooltipInteract : ButtonInteract {
    public GameObject tooltip;

    private IEnumerator tooltipAnim;

    private static readonly float waitTime = 2.0f;

    public override void OnCursorEnter() {
        base.OnCursorEnter();
        if (tooltipAnim != null) {
            StopCoroutine(tooltipAnim);
            tooltipAnim = TooltipShow();
            StartCoroutine(tooltipAnim);
        }
    }

    public override void OnCursorExited() {
        base.OnCursorExited();
        if (tooltipAnim != null) {
            StopCoroutine(tooltipAnim);
            tooltipAnim = TooltipHide();
            StartCoroutine(tooltipAnim);
        }
    }

    private IEnumerator TooltipShow() {
        yield return new WaitForSeconds(waitTime);
        tooltip.SetActive(true);
    }

    private IEnumerator TooltipHide() {
        yield return new WaitForSeconds(waitTime);
        tooltip.SetActive(false);
    }
}

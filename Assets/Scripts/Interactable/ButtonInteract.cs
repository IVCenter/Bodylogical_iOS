using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : MonoBehaviour, IInteractable {
    [Header("This button is clicked. Indicate what to happen.")]
    public UnityEvent clicked;

    private Color? originalColor;

    // Tooltip
    [Header("If no tooltip needed, leave this blank.")]
    public string tooltipTextId;
    public LocalizedParam[] tooltipTextParams;

    public GameObject tooltipPrefab;
    private GameObject tooltip;
    private IEnumerator tooltipAnim;

    // Following three variables in seconds
    private static readonly float showTime = 2.0f;
    private static readonly float hideTime = 0.5f;
    private static readonly float animationTime = 0.5f;

    private float origX;
    private float origY;

    public void OnCursorEnter() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            if (originalColor == null) {
                originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
            }

            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (!string.IsNullOrEmpty(tooltipTextId) && tooltip == null) {
            tooltip = Instantiate(tooltipPrefab, transform.parent);
            tooltip.transform.localPosition = new Vector3(0, 0, -GetComponent<Collider>().bounds.size.y / 2.0f - 3.0f);
            tooltip.transform.GetChild(0).GetChild(0).GetComponent<LocalizedText>().SetText(tooltipTextId, tooltipTextParams);
            tooltip.SetActive(false);
        }

        if (tooltip != null) {
            if (tooltipAnim != null) {
                StopCoroutine(tooltipAnim);
            }
            tooltipAnim = TooltipShow();
            StartCoroutine(tooltipAnim);
        }
    }

    public void OnCursorExited() {
        if (GetComponent<MeshRenderer>() && originalColor != null) {
            GetComponent<MeshRenderer>().material.color = (Color)originalColor;
            originalColor = null;
        }

        if (tooltip != null) {
            if (tooltipAnim != null) {
                StopCoroutine(tooltipAnim);
            }

            // OnCursorExited might be triggered when the button gets hidden.
            // In this case coroutines cannot be started, but we still wish to hide the tooltip.
            if (gameObject.activeInHierarchy) {
                tooltipAnim = TooltipHide();
                StartCoroutine(tooltipAnim);
            } else {
                tooltip.SetActive(false);
            }
        }
    }

    public virtual void OnScreenTouch(Vector2 coord) {
        clicked.Invoke();
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) {}
    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) {}
    public void OnScreenLeave(Vector2 coord) {}

    /// <summary>
    /// Displays the tooltip.
    /// </summary>
    private IEnumerator TooltipShow() {
        yield return new WaitForSeconds(showTime);
        tooltip.SetActive(true);
        // For canvases z-scale doesn't matter
        origX = tooltip.transform.localScale.x;
        origY = tooltip.transform.localScale.y;

        tooltip.transform.localScale = new Vector3(0, 0, 1);
        int stepCount = (int)(animationTime / Time.deltaTime);
        for (int i = 1; i <= stepCount; i++) {
            tooltip.transform.localScale = new Vector3(i * origX / stepCount,
                i * origY / stepCount, 1.0f);
            yield return null;
        }
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    private IEnumerator TooltipHide() {
        yield return new WaitForSeconds(hideTime);
        // For canvases z-scale doesn't matter
        int stepCount = (int)(animationTime / Time.deltaTime);
        for (int i = stepCount - 1; i >= 0; i--) {
            tooltip.transform.localScale = new Vector3(i * origX / stepCount, i * origY / stepCount, 1.0f);
            yield return null;
        }
        tooltip.SetActive(false);
        tooltip.transform.localScale = new Vector3(origX, origY, 1);
    }
}

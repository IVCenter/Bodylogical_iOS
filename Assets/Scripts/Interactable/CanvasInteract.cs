using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasInteract : TooltipInteract {
    public Image panel;

    [Header("This UI canvas is clicked. Indicate what to do next.")]
    public UnityEvent clicked;

    /// <summary>
    /// Percentage of darkness added to the original color when the canvas is hovered.
    /// </summary>
    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    public override void OnCursorEnter() {
        base.OnCursorEnter();
        if (panel != null) {
            panel.color -= darkColor;
        }
    }

    public override void OnCursorExited() {
        base.OnCursorExited();
        if (panel != null) {
            panel.color += darkColor;
        }
    }

    public override void OnScreenTouch(Vector2 coord) {
        clicked.Invoke();
    }

    public override void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }
    public override void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }
    public override void OnScreenLeave(Vector2 coord) { }
}

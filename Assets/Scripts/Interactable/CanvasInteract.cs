using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasInteract : Interactable {
    [SerializeField] private Image panel;
    [SerializeField] private UnityEvent clicked;

    /// <summary>
    /// Percentage of darkness added to the original color when the canvas is hovered.
    /// </summary>
    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    public override void OnTouchDown() {
        if (panel != null) {
            panel.color -= darkColor;
        }
    }

    public override void OnTouchUp() {
        if (panel != null) {
            panel.color += darkColor;
        }
        clicked.Invoke();
    }
}

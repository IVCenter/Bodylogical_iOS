using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class CanvasInteract : MonoBehaviour, IInteractable {
    public Image panel;

    [Header("This UI canvas is clicked. Indicate what to do next.")]
    public UnityEvent clicked;

    private Color? originalColor;

    public void OnCursorEnter() {
        if (panel != null) {
            if (originalColor == null) {
                originalColor = panel.color;
            }

            panel.color = Color.red;
        }
    }

    public void OnCursorExited() {
        if (panel != null && originalColor != null) {
            panel.color = (Color)originalColor;
            originalColor = null;
        }
    }

    public void OnScreenTouch(Vector2 coord) {
        clicked.Invoke();
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) { }
}

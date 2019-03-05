using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class DropDownOptionInteract : MonoBehaviour, IInteractable {
    public Image panel;

    [HideInInspector]
    public int index;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    [HideInInspector]
    public IntEvent clicked;

    public Color? originalColor;

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
        clicked.Invoke(index);
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) { }
}

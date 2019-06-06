using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : TooltipInteract {
    [Header("This button is clicked. Indicate what to happen.")]
    public UnityEvent clicked;

    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    // Animation
    public ComponentAnimation buttonAnimation;

    public override void OnCursorEnter() {
        base.OnCursorEnter();

        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color -= darkColor;
        }
    }

    public override void OnCursorExited() {
        base.OnCursorExited();

        if (GetComponent<MeshRenderer>()) {
            GetComponent<MeshRenderer>().material.color += darkColor;
        }
    }

    public override void OnScreenTouch(Vector2 coord) {
        if (buttonAnimation != null && !buttonAnimation.IsAnimating) {
            buttonAnimation.Invoke(clicked.Invoke);
        } else if (buttonAnimation == null) {
            clicked.Invoke();
        }
    }

    public override void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }
    public override void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }
    public override void OnScreenLeave(Vector2 coord) { }
}

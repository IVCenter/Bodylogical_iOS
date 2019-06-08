using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : Interactable {
    [Header("This button is clicked. Indicate what to happen.")]
    public UnityEvent clicked;

    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    // Animation
    public ComponentAnimation buttonAnimation;

    public override void OnTouchDown() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color -= darkColor;
        }
    }

    public override void OnTouchUp() {
        if (GetComponent<MeshRenderer>()) {
            GetComponent<MeshRenderer>().material.color += darkColor;
        }

        if (buttonAnimation != null && !buttonAnimation.IsAnimating) {
            buttonAnimation.Invoke(clicked.Invoke);
        } else if (buttonAnimation == null) {
            clicked.Invoke();
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : Interactable {
    [SerializeField] private UnityEvent clicked;

    private const float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    // Animation
    [SerializeField] private ComponentAnimation buttonAnimation;

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

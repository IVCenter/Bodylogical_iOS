using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : Interactable {
    public UnityEvent clicked;
    private MeshRenderer meshRenderer;
    private const float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void OnTouchDown() {
        if (!Enabled) {
            return;
        }

        if (meshRenderer != null) {
            meshRenderer.material.color -= darkColor;
        }
    }

    public override void OnTouchUp() {
        if (!Enabled) {
            return;
        }

        if (meshRenderer != null) {
            meshRenderer.material.color += darkColor;
        }

        clicked.Invoke();
    }
}
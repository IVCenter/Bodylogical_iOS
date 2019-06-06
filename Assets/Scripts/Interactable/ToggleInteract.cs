using UnityEngine;

public class ToggleInteract : TooltipInteract {
    public bool on;

    [Header("The value is toggled. Indicate what to do next.")]
    public CustomEvents.BoolEvent toggled;
    public LocalizedText status;
    // Toggle animation
    public ComponentAnimation toggleAnimation;

    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

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
        if (toggleAnimation != null && !toggleAnimation.IsAnimating) {
            toggleAnimation.Invoke(() => {
                Toggle();
                status.SetText(on ? "Buttons.ToggleOn" : "Buttons.ToggleOff");
                toggled.Invoke(on);
            });
        } else if (toggleAnimation == null) {
            Toggle();
            status.SetText(on ? "Buttons.ToggleOn" : "Buttons.ToggleOff");
            toggled.Invoke(on);
        }
    }

    public override void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }
    public override void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }
    public override void OnScreenLeave(Vector2 coord) { }


    /// <summary>
    /// Toggle this instance. DOES NOT invoke clicked.
    /// </summary>
    public void Toggle() {
        on = !on;
    }
}

using UnityEngine;

public class ToggleInteract : Interactable {
    public bool on;

    public CustomEvents.BoolEvent toggled;
    public LocalizedText status;
    // Toggle animation
    public ComponentAnimation toggleAnimation;

    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    public override void OnTouchDown() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color -= darkColor;
        }
    }

    public override void OnTouchUp() {
        if (GetComponent<MeshRenderer>()) {
            GetComponent<MeshRenderer>().material.color += darkColor;
        }
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

    /// <summary>
    /// Toggle this instance. DOES NOT invoke clicked.
    /// </summary>
    public void Toggle() {
        on = !on;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleInteract : MonoBehaviour, IInteractable {
    public bool isOn;

    public Image panel;

    public GameObject checkmark;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Header("The value is toggled. Indicate what to do next.")]
    public BoolEvent toggled;

    private Color? originalColor;

    #region IInteractible
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
        isOn = !isOn;
        checkmark.SetActive(isOn);
        toggled.Invoke(isOn);
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) { }


    void OnValidate() {
        checkmark.SetActive(isOn);
        // Calling toggled in editor may cause exceptions
        //toggled.Invoke(isOn);
    }
    #endregion

    /// <summary>
    /// Toggle this instance. DOES NOT invoke clicked.
    /// </summary>
    public void Toggle() {
        isOn = !isOn;
        checkmark.SetActive(isOn);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleInteract : MonoBehaviour, IInteractable {
    public bool isOn;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Header("The value is toggled. Indicate what to do next.")]
    public BoolEvent toggled;

    private Color? originalColor;

    #region IInteractible
    public void OnCursorEnter() {

    }

    public void OnCursorExited() {

    }

    public void OnScreenTouch(Vector2 coord) {

    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) { }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) { }

    #endregion

    /// <summary>
    /// Toggle this instance. DOES NOT invoke clicked.
    /// </summary>
    public void Toggle() {
        isOn = !isOn;
    }
}

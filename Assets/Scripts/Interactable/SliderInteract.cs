using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SliderInteract : MonoBehaviour, IInteractable {
    /// <summary>
    /// Current value of the slider.
    /// </summary>
    [Range(0, 1)]
    public float value;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Header("This slider is moved. Indicate what to happen.")]
    public FloatEvent changed;

    /// <summary>
    /// The left and right borders of the slider.
    /// </summary>
    public Transform left, right;

    /// <summary>
    /// Whether the user is moving the slider.
    /// </summary>
    private bool isMoving = false;
    /// <summary>
    /// color of the knob.
    /// </summary>
    private Color origin_color;
    /// <summary>
    /// Awake this instance.
    /// </summary>
    private Vector3 lastCursorPosition;

    #region Unity Routines
    void Awake() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            origin_color = GetComponent<MeshRenderer>().material.color;
        }
    }

    void OnValidate() {
        //SetSlider(value);
    }
    #endregion

    #region IInteractable
    public void OnCursorEnter() {
        //Debug.Log("Cursor Entered");
        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public void OnCursorExited() {
        //Debug.Log("Cursor Exited");
        if (!isMoving && gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color = origin_color;
        }
    }

    public void OnScreenTouch(Vector2 coord) {
        print("Screen touched");

        lastCursorPosition = CursorManager.Instance.cursor.transform.position;
        isMoving = true;
    }

    /// <summary>
    /// Uses vector angle to calculate whether the cursor has moved left or right,
    /// then move the knob accordingly.
    /// </summary>
    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) {
        Vector3 currCursorPosition = CursorManager.Instance.cursor.transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 vec1 = lastCursorPosition - cameraPosition;
        Vector3 vec2 = currCursorPosition - cameraPosition;
        float angle = Vector3.SignedAngle(vec1, vec2, Vector3.up);

        string message = "Screen pressed, angle is " + angle;
        print(message);
        SetSlider(value + angle / 10.0f);

        changed.Invoke(value);

        lastCursorPosition = currCursorPosition;
    }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) {
        print("Screen leave");

        isMoving = false;
    }
    #endregion

    #region Slider
    public void SetSlider(float val) {
        if (val < 0) {
            value = 0;
            transform.position = left.position;
        } else if (val > 1) {
            value = 1;
            transform.position = right.position;
        } else {
            value = val;
            transform.position = Vector3.Lerp(left.position, right.position, val);
        }
    }
    #endregion
}

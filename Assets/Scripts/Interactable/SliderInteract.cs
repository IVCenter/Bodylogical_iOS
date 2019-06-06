using UnityEngine;

public class SliderInteract : MonoBehaviour, IInteractable {
    /// <summary>
    /// Current value of the slider.
    /// </summary>
    [Range(0, 1)]
    public float value;

    [Header("This slider is moved. Indicate what to happen.")]
    public CustomEvents.FloatEvent changed;

    /// <summary>
    /// The left and right borders of the slider.
    /// </summary>
    public Transform left, right;

    /// <summary>
    /// Whether the user is moving the slider.
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private Vector3 lastCursorPosition;

    /// <summary>
    /// Percentage of darkness added to the original color when the canvas is hovered.
    /// </summary>
    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    #region IInteractable
    public void OnCursorEnter() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color -= darkColor;
        }
    }

    public void OnCursorExited() {
        if (!isMoving && gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color += darkColor;
        }
    }

    public void OnScreenTouch(Vector2 coord) {
        print("Screen touched slider");

        lastCursorPosition = InputManager.Instance.cursor.transform.position;
        isMoving = true;
    }

    /// <summary>
    /// Uses vector angle to calculate whether the cursor has moved left or right,
    /// then move the knob accordingly.
    /// </summary>
    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) {
        Vector3 currCursorPosition = InputManager.Instance.cursor.transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 vec1 = lastCursorPosition - cameraPosition;
        Vector3 vec2 = currCursorPosition - cameraPosition;
        float angle = Vector3.SignedAngle(vec1, vec2, Vector3.up);

        string message = "Screen pressed slider, angle is " + angle;
        print(message);
        SetSlider(value + angle / 10.0f);

        changed.Invoke(value);

        lastCursorPosition = currCursorPosition;
    }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) { }

    public void OnScreenLeave(Vector2 coord) {
        print("Screen leave slider");

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

using UnityEngine;

public class SliderInteract : Interactable {
    /// <summary>
    /// Current value of the slider.
    /// </summary>
    [Range(0, 1)] [SerializeField] private float value;

    public float Value => value;

    [SerializeField] private CustomEvents.FloatEvent changed;

    /// <summary>
    /// The left and right borders of the slider.
    /// </summary>
    [SerializeField] private Transform left, right;

    private MeshRenderer meshRenderer;

    /// <summary>
    /// Last finger position on the screen, used for sliding the knob.
    /// </summary>
    private Vector3 lastCursorPosition;

    /// <summary>
    /// Percentage of darkness added to the original color when the knob is touched.
    /// </summary>
    private const float dark = 0.4f;

    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        SetSlider(value);
    }

    #region Interactable

    public override void OnTouchDown() {
        if (meshRenderer != null) {
            meshRenderer.material.color -= darkColor;
        }

        lastCursorPosition = InputManager.Instance.WorldPos;
    }

    public override void OnTouchUp() {
        if (meshRenderer != null) {
            meshRenderer.material.color += darkColor;
        }
    }

    /// <summary>
    /// Uses vector angle to calculate whether the cursor has moved left or right,
    /// then move the knob accordingly.
    /// </summary>
    public override void OnTouchHold() {
        Vector3 currCursorPosition = InputManager.Instance.WorldPos;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 vec1 = lastCursorPosition - cameraPosition;
        Vector3 vec2 = currCursorPosition - cameraPosition;
        float angle = Vector3.SignedAngle(vec1, vec2, Vector3.up);

        SetSlider(value + angle / 15.0f);

        changed.Invoke(value);

        lastCursorPosition = currCursorPosition;
    }

    #endregion

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
}
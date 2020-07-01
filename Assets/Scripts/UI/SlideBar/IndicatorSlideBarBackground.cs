using UnityEngine;

/// <summary>
/// Controls the backgrounds of different statuses of the ribbon graph.
/// </summary>
public class IndicatorSlideBarBackground : SlideBarBackground {
    [SerializeField] private RectTransform normal, warning, upper;

    void Start() {
        if (normal == null) {
            normal = transform.Find("Normal").GetComponent<RectTransform>();
        }
        if (warning == null) {
            warning = transform.Find("Warning").GetComponent<RectTransform>();
        }
        if (upper == null) {
            upper = transform.Find("Upper").GetComponent<RectTransform>();
        }
    }

    public override void SetWarningBound() {
        warning.localPosition = new Vector3(warningBound,
          warning.localPosition.y,
          warning.localPosition.z);
        normal.sizeDelta = new Vector2(warningBound, normal.sizeDelta.y);
        warning.sizeDelta = new Vector2(100 - warningBound, warning.sizeDelta.y);
    }

    public override void SetUpperBound() {
        upper.localPosition = new Vector3(upperBound,
          upper.localPosition.y,
          upper.localPosition.z);
        warning.sizeDelta = new Vector2(upperBound - warningBound, warning.sizeDelta.y);
        upper.sizeDelta = new Vector2(100 - upperBound, upper.sizeDelta.y);
    }

    /// <summary>
    /// Switch background on/off.
    /// </summary>
    /// <param name="on">If set to <c>true</c> on.</param>
    public void ToggleBackgroundColor(bool on) {
        normal.GetComponent<ChangeableColor>().ToggleColor(on);
        warning.GetComponent<ChangeableColor>().ToggleColor(on);
        upper.GetComponent<ChangeableColor>().ToggleColor(on);
    }

    public void ToggleBackground(bool on) {
        normal.gameObject.SetActive(on);
        warning.gameObject.SetActive(on);
        upper.gameObject.SetActive(on);
    }
}

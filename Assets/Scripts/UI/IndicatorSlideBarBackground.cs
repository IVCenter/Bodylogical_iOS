using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorSlideBarBackground : SlideBarBackground {
  public RectTransform normal, warning, upper;

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
}

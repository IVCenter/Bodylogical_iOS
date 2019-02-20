using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectTransform : MonoBehaviour {
    public RectTransform rectTransform;

    public Vector2 offsetMin, offsetMax;

    private void OnValidate() {
        rectTransform.offsetMax = offsetMax;
        rectTransform.offsetMin = offsetMin;
    }
}

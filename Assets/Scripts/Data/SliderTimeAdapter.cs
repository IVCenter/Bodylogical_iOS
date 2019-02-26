using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An adapter to convert from slider values (0.0-1.0f) to time values (0-25years).
/// </summary>
public class SliderTimeAdapter : MonoBehaviour {
    public void OnSliderChanged(float value) {
        int year = (int)(value * 25);
        TimeProgressManager.Instance.UpdateYear(year);
    }
}
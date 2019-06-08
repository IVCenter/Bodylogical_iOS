using UnityEngine;

/// <summary>
/// An adapter to convert from slider values (0.0-1.0f) to time values (0-25years).
/// </summary>
public class SliderTimeAdapter : MonoBehaviour {
    public void OnSliderChanged(float value) {
        int year = (int)(value * TimeProgressManager.maxYears);
        TimeProgressManager.Instance.UpdateYear(year);
    }
}
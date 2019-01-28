using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An adapter to convert from slider values (0.0-1.0f) to time values (0-25years).
/// </summary>
public class SliderTimeAdapter : MonoBehaviour {
    public SliderInteract slider;
    public Text sliderText; // TODO: this is a bad design. Adapter should not be responsible for this.
    
    public void OnSliderChanged() {
        int value = Convert(slider.value);
        StageManager.Instance.UpdateYear(value);
        sliderText.text = value + " year";
        if (value > 1) {
            sliderText.text += "s";
        }
    }

    public int Convert(float value) {
        return (int) (value * 25);
    }
}

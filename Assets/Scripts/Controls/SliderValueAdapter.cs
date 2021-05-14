using UnityEngine;

public class SliderValueAdapter : MonoBehaviour {
    [SerializeField] private float min;
    [SerializeField] private float max;
    [SerializeField] private LocalizedText text;
    [SerializeField] private bool displayAsInteger = true;

    private SliderInteract slider;
    public SliderInteract Slider => slider == null ? slider = GetComponent<SliderInteract>() : slider;

    public float Value => min + Slider.Value * (max - min);

    public void OnValueChanged(float value) {
        text.SetText(null, new LocalizedParam(displayAsInteger ? $"{Value:0}" : $"{Value:0.00}"));
    }
}
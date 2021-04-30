using UnityEngine;

/// <summary>
/// An adapter to convert from slider values (0.0-1.0f) to time values (0-25years).
/// </summary>
public class SliderTimeAdapter : MonoBehaviour {
    public void OnSliderChanged(float value) {
        float year = value * (ArchetypeManager.Instance.Performer.ArchetypeHealth.Count - 1);
        TimeProgressManager.Instance.UpdateYear(year);
    }
}
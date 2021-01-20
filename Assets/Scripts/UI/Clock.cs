using UnityEngine;

/// <summary>
/// A 12-hour analog clock.
/// </summary>
public class Clock : MonoBehaviour {
    public RectTransform pointer;

    public void SetHour(float hour) {
        float degree = -hour * 30;
        pointer.eulerAngles = new Vector3(0, 0, degree);
    }
}

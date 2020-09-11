using UnityEngine;

/// <summary>
/// A 12-hour analog clock.
/// </summary>
public class Clock : MonoBehaviour {
    public RectTransform pointer;
    [Range(0, 24)]
    [SerializeField] private int time;

    public void Rotate(float deg) {
        pointer.Rotate(new Vector3(0, 0, -deg));
    }

    public void SetDegree(float deg) {
        pointer.eulerAngles = new Vector3(0, 0, -deg);
    }

    public void SetHour(int hour) {
        SetDegree(hour * 30);
    }

    private void OnValidate() {
        SetHour(time);
    }
}

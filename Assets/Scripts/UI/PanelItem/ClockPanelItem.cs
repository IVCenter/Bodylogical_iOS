using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains a clock and a fixed-position status.
/// </summary>
public class ClockPanelItem : PanelItem {
    [SerializeField] private Text text;
    [SerializeField] private Clock clock;
    [SerializeField] private int time;

    private void OnValidate() {
        SetValue(time);
        SetText();
    }

    public override void SetValue(float value, int index = 0) {
        time = (int)value;
        clock.SetHour(time);
        SetText();
    }

    public override void SetText() {
        text.text = time.ToString();
    }
}

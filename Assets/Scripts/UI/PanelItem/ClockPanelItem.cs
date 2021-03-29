using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains a clock and a fixed-position status.
/// </summary>
public class ClockPanelItem : PanelItem {
    [SerializeField] private Text text;
    [SerializeField] private Clock clock;
    private float time;

    public override void SetValue(int index, float value) {
        time = value;
        clock.SetHour(time);
        SetText();
    }

    public override void SetText() {
        text.text = time.ToString();
    }
}

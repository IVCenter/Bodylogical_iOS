using UnityEngine;
using UnityEngine.UI;

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
        clock.SetHour((int)value);
        SetText();
    }

    public override void SetText() {
        text.text = time.ToString();
    }
}

using UnityEngine.UI;

public class ClockPanelItem : PanelItem {
    public Text text;
    public TwoDClock clock;
    public int time;

    void OnValidate() {
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

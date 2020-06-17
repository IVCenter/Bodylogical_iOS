using UnityEngine.UI;

/// <summary>
/// Adapts a slidebar to a PanelItem.
/// </summary>
public class SliderPanelItem : PanelItem {
    public Text text;
    public SlideBarManager slideBarManager;
    public float[] values;

    public override void SetValue(float value, int index = 0) {
        values[index] = value;
        slideBarManager.SetSlideBar(index, value);
        SetText();
    }

    public override void SetText() {
        string valueText = "";

        for (int i = 0; i < values.Length; i++) {
            valueText += values[i];
            if (i != values.Length - 1) {
                valueText += " / ";
            }
        }

        text.text = valueText;
    }
}

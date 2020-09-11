using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains a slide bar and a fixed-position status text.
/// </summary>
public class SliderPanelItem : PanelItem {
    [SerializeField] private Text text;
    [SerializeField] private SlideBarManager slideBarManager;
    [SerializeField] private float[] values;

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

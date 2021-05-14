using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains a slide bar and a fixed-position status text.
/// </summary>
public class SliderPanelItem : PanelItem {
    [SerializeField] private Text text;
    [SerializeField] private SlideBarManager slideBarManager;
    /// <summary>
    /// Optional. Determines whether a specific value needs to be shown as an integer.
    /// </summary>
    [SerializeField] private bool[] displayAsInteger;
    [SerializeField] private float[] values;

    private string[] StringVals {
        get {
            string[] results = new string[values.Length];
            for (int i = 0; i < values.Length; i++) {
                results[i] = DisplayAsInteger(i) ? $"{values[i]:0}" : $"{values[i]:0.00}";
            }

            return results;
        }
    }

    private bool DisplayAsInteger(int index) => displayAsInteger != null & index >= 0 &&
                                               index < displayAsInteger.Length && displayAsInteger[index];

    public override void SetValue(int index, float value) {
        values[index] = value;
        slideBarManager.SetSlideBar(index, value);
        SetText();
    }

    public override void SetText() {
        text.text = string.Join(" / ", StringVals);
    }
}
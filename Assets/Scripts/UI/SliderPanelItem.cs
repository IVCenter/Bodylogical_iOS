using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPanelItem : PanelItem {
    public Text text;
    public SlideBarManager slideBarManager;
    public float[] values;

    /// <summary>
    /// For debugging purposes ONLY.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="index">Index.</param>
    //void OnValidate() {
    //    string valueText = "";

    //    for (int i = 0; i < values.Length; i++) {
    //        slideBarManager.SetSlideBar(i, values[i]);
    //        valueText += values[i];
    //        if (i != values.Length - 1) {
    //            valueText += " / ";
    //        }
    //    }

    //    text.text = valueText;
    //}

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

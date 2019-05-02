using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorPanelItem : PanelItem {
    /// <summary>
    /// 0: no choice
    /// 1: minimum change
    /// 2: optimum change
    /// </summary>
    public float[] values;
    public Text[] texts;
    public SlideBarManager slideBarManager;
    
    public static readonly int scale = 5;

    void OnValidate() {
        for (int i = 0; i < values.Length; i++) {
            slideBarManager.SetSlideBar(i, values[i]);
            int progress = slideBarManager.slideBars[i].progress * scale;
            texts[i].transform.localPosition = new Vector3(progress,
              texts[i].transform.localPosition.y,
              texts[i].transform.localPosition.z);
            texts[i].text = values[i].ToString();
        }
    }

    public override void SetValue(float value, int index = 0) {
        values[index] = value;
        slideBarManager.SetSlideBar(index, value);
        SetText();
    }

    public override void SetText() {
        for (int i = 0; i < values.Length; i++) {
            int progress = slideBarManager.slideBars[i].progress * scale;
            texts[i].transform.localPosition = new Vector3(progress,
              texts[i].transform.localPosition.y,
              texts[i].transform.localPosition.z);
            texts[i].text = values[i].ToString();
        }
    }
}

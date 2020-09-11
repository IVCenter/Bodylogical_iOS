using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Includes a slide bar (ideally with a pointer), and the status text is below the pointer.
/// </summary>
public class IndicatorPanelItem : PanelItem {
    /// <summary>
    /// 0: no choice
    /// 1: minimum change
    /// 2: optimum change
    /// </summary>
    [SerializeField] private float[] values;
    [SerializeField] private Text[] texts;
    public SlideBarManager slideBarManager;

    private const int scale = 5;

    private void OnValidate() {
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

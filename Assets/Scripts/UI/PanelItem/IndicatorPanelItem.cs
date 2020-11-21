using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Includes a slide bar (ideally with a pointer), and the status text is below the pointer.
/// </summary>
public class IndicatorPanelItem : PanelItem {
    [System.Serializable]
    private class DataField {
        public float value;
        public Text text;

        public void UpdateProgress(float xVal) {
            Vector3 localPos = text.transform.localPosition;
            localPos.x = xVal;
            text.transform.localPosition = localPos;
            text.text = value.ToString();
        }
    }

    [SerializeField] private DataField[] fields;

    public SlideBarManager slideBarManager;

    private const int scale = 5;

    private void OnValidate() {
        for (int i = 0; i < fields.Length; i++) {
            DataField field = fields[i];
            slideBarManager.SetSlideBar(i, field.value);
            int progress = slideBarManager.slideBars[i].progress * scale;
            field.UpdateProgress(progress);
        }
    }

    public override void SetValue(float value, int index = 0) {
        fields[index].value = value;
        slideBarManager.SetSlideBar(index, value);
        SetText();
    }

    public override void SetText() {
        for (int i = 0; i < fields.Length; i++) {
            DataField field = fields[i];
            int progress = slideBarManager.slideBars[i].progress * scale;
            field.UpdateProgress(progress);
        }
    }
}

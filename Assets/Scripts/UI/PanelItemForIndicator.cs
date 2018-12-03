using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelItemForIndicator : MonoBehaviour {
  public float[] values;
  public Text[] texts;
  public SlideBarManager slideBarManager;

  public static readonly int scale = 5;

  void OnValidate() {
    /* Set the values */
    for (int i = 0; i < values.Length; i++) {
      slideBarManager.SetSlideBar(i, values[i]);
      int progress = slideBarManager.slideBars[i].progress * scale;
      texts[i].transform.localPosition = new Vector3(progress,
        texts[i].transform.localPosition.y,
        texts[i].transform.localPosition.z);
      texts[i].text = values[i].ToString();
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class PanelItem : MonoBehaviour {
  public Text text;
  public SlideBarManager slideBarManager;
  public float[] values;

  void OnValidate() {
    string valueText = "";

    for (int i = 0; i < values.Length; i++) {
      slideBarManager.SetSlideBar(i, values[i]);
      valueText += values[i];
      if (i != values.Length - 1) {
        valueText += " / ";
      }
    }

    text.text = valueText;
  }
}

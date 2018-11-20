using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelItemForIndicator : MonoBehaviour {
  public float[] values;
  public Text[] texts;
  public SlideBarManager slideBarManager;

  public uint scale;

  void OnValidate() {
    /* Deal with sizes */
    slideBarManager.transform.Find("Background").localScale = new Vector3(scale, scale, 0);
    slideBarManager.transform.Find("Pointers").localScale = new Vector3(scale, scale, 0);
    transform.Find("Values").localPosition = new Vector3(slideBarManager.transform.localPosition.x,
      slideBarManager.transform.localPosition.y + scale * 10f,
      slideBarManager.transform.localPosition.z);

    /* Set the values */
    for (int i = 0; i < values.Length; i++) {
      slideBarManager.SetSlideBar(i, values[i]);
      int progress = slideBarManager.slideBars[i].progress * (int) scale;
      texts[i].transform.localPosition = new Vector3(progress,
        texts[i].transform.localPosition.y,
        texts[i].transform.localPosition.z);
      texts[i].text = values[i].ToString();
      texts[i].fontSize = 10 + (int) scale;
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearSlideBar : SlideBar {
  public RectTransform progress1, progress2;

  public override void SetProgress(int progress) {
    this.progress = progress;
    if (progress <= 75) {
      progress1.sizeDelta = new Vector2(progress, progress1.sizeDelta.y);
      progress2.sizeDelta = new Vector2(1, progress2.sizeDelta.y);
    } else if (progress <= 100) {
      progress1.sizeDelta = new Vector2(75, progress1.sizeDelta.y);
      progress2.sizeDelta = new Vector2(progress - 75, progress2.sizeDelta.y);
    }
  }
}

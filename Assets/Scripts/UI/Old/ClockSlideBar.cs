using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Failed attempt at converting clock to slidebar.
/// </summary>
public class ClockSlideBar : SlideBarPointer {
  public RectTransform pointer;

  public override void SetProgress(int progress) {
    float deg = progress / 100.0f * 360.0f;
    this.progress = progress;
    pointer.Rotate(new Vector3(0, 0, -deg));
  }
}

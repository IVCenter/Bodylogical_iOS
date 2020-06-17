using UnityEngine;

public class IndicatorSlideBarPointer : SlideBarPointer {
  public override void SetProgress(int progress) {
    this.progress = progress;
    transform.localPosition = new Vector3(progress, transform.localPosition.y, transform.localPosition.z);
  }
}
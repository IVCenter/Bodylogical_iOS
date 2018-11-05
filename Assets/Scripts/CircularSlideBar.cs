using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Circular slide bar with a high bar.
/// </summary>
public class CircularSlideBar : SlideBar {
  public Image progressBar1, progressBar2;

  public override void SetProgress(int progress) {
    this.progress = progress;
    if (progress <= 75) {
      progressBar1.fillAmount = progress / 100f;
      progressBar2.fillAmount = 0.01f;
    } else if (progress <= 100) {
      progressBar1.fillAmount = 0.75f;
      progressBar2.fillAmount = (progress - 75) / 100f;
    }
  }
}

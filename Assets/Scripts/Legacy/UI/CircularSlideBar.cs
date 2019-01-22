using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Circular slide bar with a high bar.
/// </summary>
public class CircularSlideBar : SlideBarPointer {
  public Image midProgressBar, highProgressBar;

  public Color normalColor;

  public override void SetProgress(int progress) {
    if (midProgressBar != null && highProgressBar != null) {
      this.progress = progress;
      if (progress <= 75) {
        midProgressBar.fillAmount = progress / 100f;
        highProgressBar.fillAmount = 0.01f;

        if (progress > 70) {
          midProgressBar.color = ((75 - progress) * normalColor - (70 - progress) * highProgressBar.color) / 5;
        } else {
          midProgressBar.color = normalColor;
        }
      } else if (progress <= 100) {
        midProgressBar.fillAmount = 0.75f;
        highProgressBar.fillAmount = (progress - 75) / 100f;

        midProgressBar.color = highProgressBar.color;
      }
    }
  }
}

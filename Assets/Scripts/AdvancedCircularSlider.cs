using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A slider with a low and high bar. 0-25 is considered low, 25-75 is
/// considered normal, and 75-100 is considered high.
/// </summary>
public class AdvancedCircularSlider : SlideBar {
  public Image lowProgressBar, midProgressBar, highProgressBar;

  public override void SetProgress(int progress) {
    this.progress = progress;
    if (progress <= 25) {
      lowProgressBar.fillAmount = (26f - progress) / 100f;
      midProgressBar.fillAmount = 0f;
      highProgressBar.fillAmount = 0.01f;
    } else if (progress <= 75) {
      lowProgressBar.fillAmount = 0.01f;
      midProgressBar.fillAmount = (progress - 25) / 100f;
      highProgressBar.fillAmount = 0.01f;
    } else {
      lowProgressBar.fillAmount = 0.01f;
      midProgressBar.fillAmount = 0.5f;
      highProgressBar.fillAmount = (progress - 75) / 100f;
    }
  }
}

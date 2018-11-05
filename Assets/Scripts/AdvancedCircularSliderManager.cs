using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for AdvancedCircularSlider.
/// </summary>
public class AdvancedCircularSliderManager : SliderManager {
  public float lowBar, highBar;
  public Color lowTextColor, normalTextColor, highTextColor;

  public override int GetPercentage(float number) {
    if (number >= lowBar && number <= highBar) {
      return (int)((50 * number + 25 * highBar - 75 * lowBar) / (highBar - lowBar));
    } else if (number < lowBar) {
      return (int)(30 / (number - lowBar + 2) + 10);
    } else { // bp > highBP
      return (int)(90 - 10 * highBar / number);
    }
  }
}

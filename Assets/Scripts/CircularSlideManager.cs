using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSlideManager : SliderManager {
  public float highBar;
  public Color normalTextColor, highTextColor;

  public override int GetPercentage(float number) {
    if (number <= highBar) {
      return (int)(75 * number / highBar);
    }  else { // bp > highBP
      return (int)(90 - 10 * highBar / number);
    }
  }
}

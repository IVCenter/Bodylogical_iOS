﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for normal circular slide bars (with high bars only).
/// </summary>
public class CircularSlideBarManager : SlideBarManager {
  public float[] highBars;

  public override int GetPercentage(int index, float number) {
    if (number <= highBars[index]) {
      return (int)(75 * number / highBars[index]);
    } else { // bp > highBP
      return (int)(90 - 10 * highBars[index] / number);
    }
  }

  public override NumberStatus GetStatus(int index = -1) {
    if (index == -1) {
      for (int i = 0; i < values.Count; i++) {
        if (values[i] > highBars[i]) {
          return NumberStatus.HIGH;
        }
      }
    } else {
      if (values[index] > highBars[index]) {
        return NumberStatus.HIGH;
      }
    }
    return NumberStatus.NORMAL;
  }
}
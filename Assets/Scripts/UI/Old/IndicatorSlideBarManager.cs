using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorSlideBarManager : SlideBarManager {
  public bool useUniformBar;
  public float[] highBars;

  public override int GetPercentage(int index, float number) {
    if (useUniformBar) {
      index = 0;
    }

    if (number <= highBars[index]) {
      return (int)(75 * number / highBars[index]);
    } else {
      return (int)(100 - 25 * highBars[index] / number);
    }
  }

  public override NumberStatus GetStatus(int index = -1) {
    if (index == -1) {
      for (int i = 0; i < values.Count; i++) {
        int cmpIndex = useUniformBar ? 0 : i;
        if (values[i] > highBars[cmpIndex]) {
          return NumberStatus.HIGH;
        }
      }
    } else {
      if (useUniformBar) {
        index = 0;
      }
      if (values[index] > highBars[index]) {
        return NumberStatus.HIGH;
      }
    }
    return NumberStatus.NORMAL;
  }
}

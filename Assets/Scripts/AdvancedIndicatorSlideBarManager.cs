using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedIndicatorSlideBarManager : SlideBarManager {
  public bool useUniformBar;
  public float[] lowBars;
  public float[] highBars;

  public override int GetPercentage(int index, float number) {
    if (useUniformBar) {
      index = 0;
    }
    // Linear interpolation
    if (number >= lowBars[index] && number <= highBars[index]) {
      return (int)((50 * number + 25 * highBars[index] - 75 * lowBars[index]) /
        (highBars[index] - lowBars[index]));
    // Format for calculation: prog = -a / (number - b).
    // b needs to be larger than lowBars[index].
    } else if (number < lowBars[index]) {
      return (int)(-2.5f * lowBars[index] / (number - 1.1f * lowBars[index]));
    // Format for calculation: prog = 100 - a / (number - b).
    // b needs to be smaller than highBars[index].
    } else {
      return (int)(100 - 2.5 * highBars[index] / (number - 0.9 * highBars[index]));
    }
  }

  /// <summary>
  /// Get the the status of the specific/all slide bar.
  /// </summary>
  /// <param name="index">When set to -1, get a comprehensive status that
  /// combines the statuses of all slide bars. </param>
  /// <returns></returns>
  public override HealthStatus GetStatus(int index = -1) {
    int high = 0, low = 0;
    if (index == -1) {
      for (int i = 0; i < values.Count; i++) {
        int cmpIndex = useUniformBar ? 0 : i;
        if (values[i] > highBars[cmpIndex]) {
          high++;
        } else if (values[i] < lowBars[cmpIndex]) {
          low++;
        }
      }
    } else {
      if (useUniformBar) {
        index = 0;
      }

      if (values[index] > highBars[index]) {
        high++;
      } else if (values[index] < lowBars[index]) {
        low++;
      }
    }

    if (high == 0 && low == 0) {
      return HealthStatus.NORMAL;
    } else if (high > 0 && low > 0) {
      return HealthStatus.ABNORMAL;
    } else if (high > 0) {
      return HealthStatus.HIGH;
    } else { // low > 0
      return HealthStatus.LOW;
    }
  }
}

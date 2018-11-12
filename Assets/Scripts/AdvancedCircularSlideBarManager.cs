using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for AdvancedCircularSlideBar.
/// </summary>
public class AdvancedCircularSlideBarManager : SlideBarManager {
  public float[] lowBars;
  public float[] highBars;

  public override int GetPercentage(int index, float number) {
    if (number >= lowBars[index] && number <= highBars[index]) {
      return (int)((50 * number + 25 * highBars[index] - 75 * lowBars[index]) /
        (highBars[index] - lowBars[index]));
    } else if (number < lowBars[index]) {
      return (int)(30 / (number - lowBars[index] + 2) + 10);
    } else { // bp > highBP
      return (int)(90 - 10 * highBars[index] / number);
    }
  }

  public override HealthStatus GetStatus(int index = -1) {
    int high = 0, low = 0;
    if (index == -1) {
      for (int i = 0; i < values.Count; i++) {
        if (values[i] > highBars[i]) {
          high++;
        } else if (values[i] < lowBars[i]) {
          low++;
        }
      }
    } else {
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

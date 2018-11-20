using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SlideBarManager : MonoBehaviour {
  public SlideBar[] slideBars;
  public Text status;

  protected List<float> values = new List<float>();

  private static readonly Dictionary<HealthStatus, Color> colors = new Dictionary<HealthStatus, Color> {
    { HealthStatus.NORMAL, Color.white },
    { HealthStatus.HIGH, Color.red },
    { HealthStatus.LOW, Color.blue },
    { HealthStatus.ABNORMAL, Color.yellow }
  };

  public void SetSlideBar(int index, float number) {
    if (values.Count > index) {
      values[index] = number;
    } else {
      values.Insert(index, number);
    }
    slideBars[index].SetProgress(GetPercentage(index, number));

    if (status != null) {
      HealthStatus healthStatus = GetStatus();
      status.text = healthStatus.ToString();
      status.color = colors[healthStatus];
    }
  }

  /// <summary>
  /// Increase the slide bar as time progresses.
  /// </summary>
  public void Interpolate() {
    for (int i = 0; i < slideBars.Length; i++) { 
      StartCoroutine(slideBars[i].Interpolate(GetPercentage(i, values[i])));
    }
  }

  public abstract int GetPercentage(int index, float number);

  /// <summary>
  /// Get the the status of the specific/all slide bar.
  /// </summary>
  /// <param name="index">When set to -1, get a comprehensive status that
  /// combines the statuses of all slide bars. </param>
  /// <returns></returns>
  public abstract HealthStatus GetStatus(int index = -1);
}

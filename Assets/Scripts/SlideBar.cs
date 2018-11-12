using System.Collections;
using UnityEngine;

public abstract class SlideBar : MonoBehaviour {
  [Range(0, 100)]
  public int progress;

  /// <summary>
  /// Increase the progress by 1.
  /// </summary>
  public void Increase() {
    if (progress < 100) {
      progress++;
      SetProgress(progress);
    }
  }

  /// <summary>
  /// Sets the progress of the slide bar.
  /// </summary>
  /// <param name="progress">A number between 0 to 100.</param>
  public abstract void SetProgress(int progress);

  void OnValidate() {
    SetProgress(progress);
  }

  /// <summary>
  /// TODO: have some cool effects!
  /// </summary>
  /// <param name="progress"></param>
  /// <param name="time">time in ms</param>
  /// <returns></returns>
  public IEnumerator Interpolate(int progress, int time = 100) {
    throw new System.NotImplementedException();
  }
}

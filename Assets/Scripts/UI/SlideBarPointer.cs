using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the pointer of the slidebar. Indicates progress.
/// </summary>
public abstract class SlideBarPointer : MonoBehaviour {
  [Range(0, 100)]
  public int progress;

  void OnValidate() {
    SetProgress(progress);
  }

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
  /// Slowly interpolate the progress.
  /// </summary>
  /// <param name="progress"></param>
  /// <param name="time">time in ms</param>
  /// <returns></returns>
  public IEnumerator Interpolate(int progress) {
    this.progress = 0;
    while (this.progress != progress) {
      Increase();
      yield return new WaitForFixedUpdate();
    }
  }

  /// <summary>
  /// Sets the progress of the slide bar. Also sets the color of the progress bar.
  /// </summary>
  /// <param name="progress">A number between 0 to 100.</param>
  public abstract void SetProgress(int progress);
}

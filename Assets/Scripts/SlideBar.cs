using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlideBar : MonoBehaviour {
  [Range(0, 100)]
  public int progress;

  /// <summary>
  /// Increase the progress by 1.
  /// </summary>
  public void Increase() {
    SetProgress(progress);
    if (progress < 100) {
      progress++;
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
}

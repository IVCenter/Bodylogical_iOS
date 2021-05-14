using UnityEngine;

/// <summary>
/// Controls the pointer of the slidebar. Indicates progress.
/// </summary>
public abstract class SlideBar : MonoBehaviour {
    [Range(0, 100)] public int progress;

    private void OnValidate() {
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
    /// Sets the progress of the slide bar. Also sets the color of the progress bar.
    /// </summary>
    /// <param name="progress">A number between 0 to 100.</param>
    public abstract void SetProgress(int progress);
}
using UnityEngine;

/// <summary>
/// Controls the background of the slide bar. Indicates bounds.
/// </summary>
public abstract class SlideBarBackground : MonoBehaviour {
    [Range(0, 100)]
    public int warningBound, upperBound;

    public abstract void SetWarningBound();
    public abstract void SetUpperBound();
}
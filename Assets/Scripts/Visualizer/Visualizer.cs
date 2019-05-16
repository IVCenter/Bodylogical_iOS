using UnityEngine;

public abstract class Visualizer : MonoBehaviour {
    /// <summary>
    /// Key for localization. Also for denoting name.
    /// </summary>
    public abstract string VisualizerKey { get; }
    public abstract HealthStatus Status { get; set; }
    /// <summary>
    /// Initialize() is not required by every initializer, so set to virtual.
    /// </summary>
    public virtual void Initialize() { }
    /// <summary>
    /// Performs the visualization given the health data and choice.
    /// </summary>
    /// <returns>If the health status changed for the year, return true.
    /// Otherwise, return false.</returns>
    /// <param name="index">Index in LongTermHealth. Relates to character age.</param>
    /// <param name="choice">Path choice.</param>
    public abstract bool Visualize(int index, HealthChoice choice);
    /// <summary>
    /// Pauses the animation, if any.
    /// </summary>
    public abstract void Pause();
}

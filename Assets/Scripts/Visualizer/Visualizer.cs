using UnityEngine;

public abstract class Visualizer : MonoBehaviour {
    /// <summary>
    /// Key for localization. Also for denoting name.
    /// </summary>
    public abstract string VisualizerKey { get; }
    public abstract HealthStatus Status { get; set; }
    /// <summary>
    /// Performs the visualization given the health data and choice.
    /// </summary>
    /// <returns>If the health status changed for the year, return true.
    /// Otherwise, return false.</returns>
    /// <param name="index">Index in LongTermHealth. If it is a non-integer,
    /// it means lerping between two indices.</param>
    /// <param name="choice">Path choice.</param>
    public abstract bool Visualize(float index, HealthChoice choice);
    /// <summary>
    /// Stops the animation, if any.
    /// </summary>
    public virtual void Stop() { }

    /// <summary>
    /// Resets the visualization.
    /// </summary>
    public virtual void ResetVisualizer() {}
}

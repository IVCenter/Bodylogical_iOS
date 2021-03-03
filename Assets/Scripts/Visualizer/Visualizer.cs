using UnityEngine;

public abstract class Visualizer : MonoBehaviour {
    protected ArchetypePerformer performer;
    
    public virtual void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
    }
    
    /// <summary>
    /// Performs the visualization given the health data and choice.
    /// </summary>
    /// <param name="index">Index in LongTermHealth. If it is a non-integer,
    /// it means lerping between two indices.</param>
    /// <param name="choice">Path choice.</param>
    /// <returns>If the health status changed for the year, return true.
    /// Otherwise, return false.</returns>
    public abstract bool Visualize(float index, HealthChoice choice);

    /// <summary>
    /// Stops the animation, if any.
    /// </summary>
    public virtual void Stop() { }

    /// <summary>
    /// Resets the visualization.
    /// </summary>
    public virtual void ResetVisualizer() { }
}
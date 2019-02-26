using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Visualizer : MonoBehaviour {
    public abstract HealthStatus Status { get; set; }
    /// <summary>
    /// Initialize() is not required by every initializer, so set to virtual.
    /// </summary>
    public virtual void Initialize() { }
    public abstract bool Visualize(int index, HealthChoice choice);
    public abstract void Pause();
}

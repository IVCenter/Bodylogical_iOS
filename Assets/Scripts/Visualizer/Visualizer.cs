using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Visualizer : MonoBehaviour {
    public abstract HealthStatus Status { get; set; }
    public abstract bool Visualize(int index, HealthChoice choice);
}

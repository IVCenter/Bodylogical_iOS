using System.Collections;
using UnityEngine;

public abstract class PropAnimation : MonoBehaviour {
    public float Speed { get; set; }
    
    /// <summary>
    /// Uses the Speed property to perform the necessary animation.
    /// </summary>
    public abstract IEnumerator Animate();
}

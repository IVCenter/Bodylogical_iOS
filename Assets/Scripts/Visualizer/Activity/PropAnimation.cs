using System.Collections;
using UnityEngine;


public abstract class PropAnimation : MonoBehaviour {
    public float speed;
    
    /// <summary>
    /// Uses the speed variable to perform the necessary animation.
    /// </summary>
    public abstract IEnumerator Animate();
}

using System.Collections;
using UnityEngine;

public abstract class ComponentAnimation : MonoBehaviour {
    protected IEnumerator anim;

    public void Invoke() {
        if (anim == null) {
            anim = Animate();
            StartCoroutine(anim);
        }
    }

    public void Stop() {
        StopCoroutine(anim);
    }

    /// <summary>
    /// The actual animation.
    /// **NOTICE**: always set anim to null after animation is complete!
    /// </summary>
    /// <returns>The animate.</returns>
    public abstract IEnumerator Animate();
}

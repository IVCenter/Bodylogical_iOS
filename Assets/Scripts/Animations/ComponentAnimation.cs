using System.Collections;
using UnityEngine;

public abstract class ComponentAnimation : MonoBehaviour {
    protected IEnumerator anim;

    public bool IsAnimating {
        get {
            return anim != null;
        }
    }

    public void Invoke(System.Action callback = null) {
        if (anim == null) {
            anim = Animate(callback);
            StartCoroutine(anim);
        }
    }

    public void Stop() {
        StopCoroutine(anim);
        anim = null;
    }

    /// <summary>
    /// The actual animation.
    /// **NOTICE**: always set anim to null after animation is complete!
    /// </summary>
    /// <param name="callback">A callback function to be executed when
    /// the animation is completed. Notice that this will not get called if Stop()
    /// is called to terminate in advanced.</param>
    /// <returns>The animation enumerator.</returns>
    public abstract IEnumerator Animate(System.Action callback);
}

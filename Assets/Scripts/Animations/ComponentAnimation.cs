using System.Collections;
using UnityEngine;

public abstract class ComponentAnimation : MonoBehaviour {
    protected IEnumerator anim;

    public bool IsAnimating {
        get {
            return anim != null;
        }
    }

    /// <summary>
    /// Calls the normal animation. Should be the default interaction.
    /// </summary>
    /// <param name="callback">Callback function executed after the animation
    /// is completed.</param>
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
    /// **NOTICE FOR IMPLEMENTATIONS**:
    /// 1. Remember to invoke callback if it is not null;
    /// 2. Remember to set anim to null after the animation is complete;
    /// the boolean value IsAnimating depends on whether anim is null, and not
    /// properly resetting anim would prevent the animation from being executed!
    /// </summary>
    /// <param name="callback">A callback function to be executed when
    /// the animation is completed. Notice that this will not get called if Stop()
    /// is called to terminate in advanced.</param>
    /// <returns>The animation enumerator.</returns>
    public abstract IEnumerator Animate(System.Action callback);

    /// <summary>
    /// Jumps to the state when the animation is complete.
    /// Should not be used for normal interactions; only use it for in-app logic!
    /// </summary>
    public abstract void Jump();
}

using UnityEngine;
using UnityEngine.Events;

public class Switcher : MonoBehaviour {
    private static readonly int numOptions = 3;

    // 0: top, 1: left, 2: right
    public int currOption;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    public IntEvent changed;

    public ComponentAnimation switcherAnimation;

    public void ChangeOption() {
        if (switcherAnimation == null || !switcherAnimation.IsAnimating) {
            currOption = (currOption + 1) % numOptions;
            changed.Invoke(currOption);
            switcherAnimation.Invoke();
        }
    }
}

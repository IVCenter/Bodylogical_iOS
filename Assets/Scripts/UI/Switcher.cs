using UnityEngine;

public class Switcher : MonoBehaviour {
    private static readonly int numOptions = 3;

    // 0: top, 1: left, 2: right
    public int currOption;

    public CustomEvents.IntEvent changed;

    public ComponentAnimation switcherAnimation;

    public void ChangeOption() {
        if (switcherAnimation != null && !switcherAnimation.IsAnimating) {
            switcherAnimation.Invoke(() => {
                currOption = (currOption + 1) % numOptions;
                changed.Invoke(currOption);
            });
        } else if (switcherAnimation == null) {
            currOption = (currOption + 1) % numOptions;
            changed.Invoke(currOption);
        }
    }
    // TODO
    public void Switch(int index) {

    }
}

using UnityEngine;

public class Switcher : MonoBehaviour {
    public int numOptions;
    public int currOption;
    public CustomEvents.IntEvent changed;
    public ComponentAnimation switcherAnimation;

    public void ChangeOption() {
        if (switcherAnimation != null && !switcherAnimation.IsAnimating) {
            // Toggle needs to go first because animation depends on option.
            currOption = (currOption + 1) % numOptions;
            switcherAnimation.Invoke(() => {
                changed.Invoke(currOption);
            });
        } else if (switcherAnimation == null) {
            currOption = (currOption + 1) % numOptions;
            changed.Invoke(currOption);
        }
    }

    public void Switch(int index) {
        currOption = index;
        if (switcherAnimation != null) {
            switcherAnimation.Jump();
        }
    }
}

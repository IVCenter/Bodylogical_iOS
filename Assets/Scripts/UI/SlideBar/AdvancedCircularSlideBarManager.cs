using UnityEngine;

/// <summary>
/// Controller for AdvancedCircularSlideBar.
/// </summary>
public class AdvancedCircularSlideBarManager : SlideBarManager {
    [SerializeField] private float[] lowBars;
    [SerializeField] private float[] highBars;

    public override int GetPercentage(int index, float number) {
        // Linear interpolation
        if (number >= lowBars[index] && number <= highBars[index]) {
            return (int) ((50 * number + 25 * highBars[index] - 75 * lowBars[index]) /
                          (highBars[index] - lowBars[index]));
            // Format for calculation: prog = -a / (number - b).
            // b needs to be larger than lowBars[index].
        }
        else if (number < lowBars[index]) {
            return (int) (-2.5f * lowBars[index] / (number - 1.1f * lowBars[index]));
            // Format for calculation: prog = 100 - a / (number - b).
            // b needs to be smaller than highBars[index].
        }
        else {
            return (int) (100 - 2.5 * highBars[index] / (number - 0.9 * highBars[index]));
        }
    }

    public override NumberStatus GetStatus(int index = -1) {
        int high = 0, low = 0;
        if (index == -1) {
            for (int i = 0; i < values.Count; i++) {
                if (values[i] > highBars[i]) {
                    high++;
                }
                else if (values[i] < lowBars[i]) {
                    low++;
                }
            }
        }
        else {
            if (values[index] > highBars[index]) {
                high++;
            }
            else if (values[index] < lowBars[index]) {
                low++;
            }
        }

        if (high == 0 && low == 0) {
            return NumberStatus.Normal;
        }
        else if (high > 0) {
            return NumberStatus.Warning;
        }
        else {
            // low > 0
            return NumberStatus.Danger;
        }
    }
}
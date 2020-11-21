using UnityEngine;

/// <summary>
/// Controller for AdvancedCircularSlideBar.
/// </summary>
public class AdvancedCircularSlideBarManager : SlideBarManager {
    [System.Serializable]
    private struct Bar {
        public float low;
        public float high;
    }

    [SerializeField] private Bar[] bars;

    public override int GetPercentage(int index, float number) {
        Bar bar = bars[index];
        
        // Linear interpolation
        if (number >= bar.low && number <= bar.high) {
            return (int) ((50 * number + 25 * bar.high - 75 * bar.low) /
                          (bar.high - bar.low));
            // Format for calculation: prog = -a / (number - b).
            // b needs to be larger than bar.low.
        }

        if (number < bar.low) {
            return (int) (-2.5f * bar.low / (number - 1.1f * bar.low));
            // Format for calculation: prog = 100 - a / (number - b).
            // b needs to be smaller than bar.high.
        }  
            
        return (int) (100 - 2.5 * bar.high / (number - 0.9 * bar.high));
    }

    public override NumberStatus GetStatus(int index = -1) {
        int high = 0, low = 0;
        if (index == -1) {
            for (int i = 0; i < values.Count; i++) {
                if (values[i] > bars[i].high) {
                    high++;
                } else if (values[i] < bars[i].low) {
                    low++;
                }
            }
        } else {
            if (values[index] > bars[index].high) {
                high++;
            } else if (values[index] < bars[index].low) {
                low++;
            }
        }

        if (high == 0 && low == 0) {
            return NumberStatus.Normal;
        }

        if (high > 0) {
            return NumberStatus.Warning;
        }

        // low > 0
        return NumberStatus.Danger;
    }
}
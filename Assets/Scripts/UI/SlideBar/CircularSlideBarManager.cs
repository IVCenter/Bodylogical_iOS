using UnityEngine;

/// <summary>
/// Manager for circular slide bars with only a warning label. No danger or max values are provided.
/// </summary>
public class CircularSlideBarManager : SlideBarManager {
    [System.Serializable]
    private struct Val {
        public float min, warning, max;
    }

    [SerializeField] private Val[] vals;

    protected override int GetPercentage(int index, float number) {
        Val val = vals[index];
        number = Mathf.Clamp(number, val.min, val.max);

        if (number <= val.warning) {
            return Mathf.RoundToInt(75 * (number - val.min) / (val.warning - val.min));
        }

        return Mathf.RoundToInt((25 * number + 75 * val.max - 100 * val.warning) / (val.max - val.warning));
    }

    /// <summary>
    /// Retrieves the status of a specific slide bar.
    /// If the index is -1, get the overall status by using the worst status of all bars.
    /// </summary>
    protected override NumberStatus GetStatus(int index = -1) {
        if (index == -1) {
            for (int i = 0; i < values.Count; i++) {
                if (values[i] > vals[i].warning) {
                    return NumberStatus.Warning;
                }
            }
        } else {
            if (values[index] > vals[index].warning) {
                return NumberStatus.Warning;
            }
        }

        return NumberStatus.Normal;
    }
}
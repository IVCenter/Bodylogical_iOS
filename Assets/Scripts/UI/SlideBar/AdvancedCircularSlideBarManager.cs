using UnityEngine;

/// <summary>
/// Controller for AdvancedCircularSlideBar.
/// </summary>
public class AdvancedCircularSlideBarManager : SlideBarManager {
    [System.Serializable]
    private struct Val {
        public float min, warning, danger, max;
    }

    [SerializeField] private Val[] vals;

    protected override int GetPercentage(int index, float number) {
        Val val = vals[index];
        number = Mathf.Clamp(number, val.min, val.max);

        if (number <= val.warning) {
            return Mathf.RoundToInt(50 * (number - val.min) / (val.warning - val.min));
        }

        if (number <= val.danger) {
            return Mathf.RoundToInt((25 * number + 50 * val.danger - 75 * val.warning) / (val.danger - val.warning));
        }

        return Mathf.RoundToInt((25 * number + 75 * val.max - 100 * val.danger) / (val.max - val.danger));
    }

    /// <summary>
    /// Retrieves the status of a specific slide bar.
    /// If the index is -1, get the overall status by using the worst status of all bars.
    /// </summary>
    protected override NumberStatus GetStatus(int index = -1) {
        int danger = 0, warning = 0;
        if (index == -1) {
            for (int i = 0; i < values.Count; i++) {
                if (values[i] > vals[i].danger) {
                    danger++;
                } else if (values[i] > vals[i].warning) {
                    warning++;
                }
            }
        } else {
            if (values[index] > vals[index].danger) {
                danger++;
            } else if (values[index] > vals[index].warning) {
                warning++;
            }
        }

        if (danger > 0) {
            return NumberStatus.Danger;
        }

        if (warning > 0) {
            return NumberStatus.Warning;
        }

        return NumberStatus.Normal;
    }
}
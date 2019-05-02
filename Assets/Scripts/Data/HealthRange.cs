using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores range data for one specific health category.
/// </summary>
public class HealthRange : MonoBehaviour {
    public HealthType type;
    /// <summary>
    /// if set to true, a different set of warnning/upper threshholds are used for different genders.
    /// The non-alt is for male, while alt is for female.
    /// </summary>
    public bool useAlt;
    public float min, max, warning, upper, warningAlt, upperAlt;

    void Start() {
        if (!useAlt) {
            warningAlt = warning;
            upperAlt = upper;
        }
    }
    /// <summary>
    /// Gets a health point depending on the min, max, warning and upper values.
    /// The range from min to max is separated into three subranges:
    /// min - warning: 100 - 60
    /// warning - upper: 60 - 30
    /// upper - max: 30 - 0
    /// </summary>
    /// <returns>Health point, from 0 to 100.</returns>
    /// <param name="value">value of the biometric.</param>
    /// <param name="alt">if true, use the alt values.</param>
    public int CalculatePoint(float value, bool alt = false) {
        // the values used for calculation
        float calcWarning = alt ? warningAlt : warning;
        float calcUpper = alt ? upperAlt : upper;

        if (value < min) {
            return 100;
        }

        if (value  < calcWarning) { // 100 - 60
            return (int)((40 * value + 60 * min - 100 * calcWarning) / (min - calcWarning));
        }

        if (value < calcUpper) { // 60 - 30
            return (int)((30 * value + 30 * calcWarning - 60 * calcUpper) / (calcWarning - calcUpper));
        }

        if (value < max) {
            return (int)((30 * value - 30 * max) / (calcUpper - max));
        }

        return 0; // value >= max
    }
}
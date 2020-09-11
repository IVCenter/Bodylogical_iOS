/// <summary>
/// Stores range data for one specific health category.
/// </summary>
[System.Serializable]
public class HealthRange {
    public HealthType type;
    /// <summary>
    /// if set to true, a different set of warnning/upper threshholds are used for different genders.
    /// The non-alt is for male, while alt is for female.
    /// </summary>
    public Gender gender;
    public float min, max, warning, upper;

    /// <summary>
    /// Gets a health point depending on the min, max, warning and upper values.
    /// The range from min to max is separated into three subranges:
    /// min - warning: 100 - 60
    /// warning - upper: 60 - 30
    /// upper - max: 30 - 0
    /// </summary>
    /// <returns>Health point, from 0 to 100.</returns>
    /// <param name="value">value of the biometric.</param>
    public int CalculatePoint(float value) {
        if (value < min) {
            return 100;
        }

        if (value  < warning) { // 100 ~ 60
            return (int)((40 * value + 60 * min - 100 * warning) / (min - warning));
        }

        if (value < upper) { // 60 ~ 30
            return (int)((30 * value + 30 * warning - 60 * upper) / (warning - upper));
        }

        if (value < max) {
            return (int)((30 * value - 30 * max) / (upper - max));
        }

        return 0; // value >= max
    }
}
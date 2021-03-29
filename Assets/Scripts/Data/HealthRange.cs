/// <summary>
/// Stores range data for one specific health category.
/// </summary>
[System.Serializable]
public class HealthRange {
    public HealthType type;
    public Gender gender;
    public float min, max, warning, danger;

    /// <summary>
    /// Calculates a health point based on the following criteria:
    /// min - warning: 100 - 60
    /// warning - danger: 60 - 30
    /// danger - max: 30 - 0
    /// </summary>
    /// <returns>Health point, from 0 to 100.</returns>
    /// <param name="value">value of the biometric.</param>
    public int CalculatePoint(float value) {
        if (value < min) {
            return 100;
        }

        if (value < warning) { // 100 ~ 60
            return (int)((40 * value + 60 * min - 100 * warning) / (min - warning));
        }

        if (value < danger) { // 60 ~ 30
            return (int)((30 * value + 30 * warning - 60 * danger) / (warning - danger));
        }

        if (value < max) {
            return (int)((30 * value - 30 * max) / (danger - max));
        }

        return 0; // value >= max
    }
}
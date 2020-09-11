/// <summary>
/// Utility methods for calculating health.
/// </summary>
public static class HealthUtil {
    public static HealthStatus CalculateStatus(int point) {
        if (point < 40) {
            return HealthStatus.Bad;
        }

        if (point < 60) {
            return HealthStatus.Moderate;
        }

        return HealthStatus.Good;
    }
    
    public static int CalculatePoint(HealthType type, Gender gender, float value) {
        HealthRange range = RangeLoader.Instance.GetRange(type, gender);
        if (value < range.min) {
            return 100;
        }

        if (value < range.warning) { // 100 ~ 60
            return (int)((40 * value + 60 * range.min - 100 * range.warning) / (range.min - range.warning));
        }

        if (value < range.upper) { // 60 ~ 30
            return (int)((30 * value + 30 * range.warning - 60 * range.upper) / (range.warning - range.upper));
        }

        if (value < range.max) {
            return (int)((30 * value - 30 * range.max) / (range.upper - range.max));
        }

        return 0; // value >= max
    }
}

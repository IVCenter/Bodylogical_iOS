/// <summary>
/// Utility methods for calculating health.
/// </summary>
public static class HealthUtil {
    public static HealthStatus CalculateStatus(int point) {
        if (point < 30) {
            return HealthStatus.Bad;
        }

        if (point < 60) {
            return HealthStatus.Moderate;
        }

        return HealthStatus.Good;
    }
    
    public static int CalculatePoint(HealthType type, Gender gender, float value) {
        HealthRange range = RangeLoader.Instance.GetRange(type, gender);
        return range.CalculatePoint(value);
    }
}

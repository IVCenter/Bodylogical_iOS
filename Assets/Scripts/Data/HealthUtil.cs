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
}

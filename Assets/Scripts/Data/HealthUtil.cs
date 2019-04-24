public static class HealthUtil {
    public static HealthStatus CalculateStatus(int point) {
        if (point < 50) { // TODO: FOR DEMOING PURPOSES ONLY. CHANGE AFTER DEMO.
            return HealthStatus.Bad;
        }

        if (point < 60) {
            return HealthStatus.Intermediate;
        }

        return HealthStatus.Good;
    }
}

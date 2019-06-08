using System.Collections.Generic;

public static class LocalizationDicts {
    public static readonly Dictionary<HealthStatus, string> statuses = new Dictionary<HealthStatus, string> {
        { HealthStatus.Bad, "General.StatusBad" },
        { HealthStatus.Moderate, "General.StatusModerate" },
        { HealthStatus.Good, "General.StatusGood" }
    };
}

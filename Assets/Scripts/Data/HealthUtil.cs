using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Utility methods for calculating health.
/// </summary>
public static class HealthUtil {
    private static List<HealthRange> ranges;

    private static List<HealthRange> Ranges => ranges ?? (ranges = DataLoader.LoadRanges());

    private static Dictionary<HealthChoice, Lifestyle> lifestyles;

    public static Dictionary<HealthChoice, Lifestyle> Lifestyles =>
        lifestyles ?? (lifestyles = DataLoader.LoadLifestyles().ToDictionary(l => l.choice, l => l));

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
        HealthRange range = GetRange(type, gender);
        return range.CalculatePoint(value);
    }

    public static HealthRange GetRange(HealthType type, Gender gender = Gender.Either) {
        var selectedRanges = from r in Ranges
            where r.type == type && (r.gender == gender || r.gender == Gender.Either)
            select r;

        return selectedRanges.First();
    }
}
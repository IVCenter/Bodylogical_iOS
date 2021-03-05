using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LongTermHealth {
    public List<Health> healths;

    public void PopulateFromJson(JArray array) {
        healths = (from obj in array select Health.FromJson((JObject) obj)).ToList();
    }

    /// <summary>
    /// Gives an overall health point.
    /// </summary>
    /// <returns>The health.</returns>
    /// <param name="index">Index.</param>
    /// <param name="gender">Specifies which set of data to look for.</param>
    public int CalculateHealth(float index, Gender gender) {
        Health floored = healths[Mathf.FloorToInt(index)];
        Health ceiled = healths[Mathf.CeilToInt(index)];

        int floorSum = (from entry in floored.values
                select HealthUtil.CalculatePoint(entry.Key, gender, entry.Value))
            .Sum();
        int ceilSum = (from entry in ceiled.values
                select HealthUtil.CalculatePoint(entry.Key, gender, entry.Value))
            .Sum();

        return Mathf.RoundToInt(Mathf.Lerp(floorSum, ceilSum, index % 1) / healths[0].values.Count);
    }

    /// <summary>
    /// Gives a health score for the selected types.
    /// </summary>
    /// <param name="index">Index.</param>
    /// <param name="gender">Specifies which set of data to look for.</param>
    /// <param name="types">Health types to use.</param>
    /// <returns>The health score for the specified index and gender.</returns>
    public int CalculateHealth(float index, Gender gender, params HealthType[] types) {
        HashSet<HealthType> typeSet = new HashSet<HealthType>(types);

        Health floored = healths[Mathf.FloorToInt(index)];
        Health ceiled = healths[Mathf.CeilToInt(index)];

        int floorSum = (from entry in floored.values
                where typeSet.Contains(entry.Key)
                select HealthUtil.CalculatePoint(entry.Key, gender, entry.Value))
            .Sum();
        int ceilSum = (from entry in ceiled.values
                where typeSet.Contains(entry.Key)
                select HealthUtil.CalculatePoint(entry.Key, gender, entry.Value))
            .Sum();

        return Mathf.RoundToInt(Mathf.Lerp(floorSum, ceilSum, index % 1) / types.Length);
    }
}
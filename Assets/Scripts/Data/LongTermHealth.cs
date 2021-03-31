using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongTermHealth : IEnumerable<Health> {
    public HealthChoice choice = HealthChoice.Custom;
    public List<Health> healths;
    public int Count => healths.Count;

    public Health this[int i] => healths[i];

    public IEnumerator<Health> GetEnumerator() {
        foreach (Health health in healths) {
            yield return health;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
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
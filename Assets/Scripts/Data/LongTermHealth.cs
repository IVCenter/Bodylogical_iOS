using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongTermHealth {
    /// <summary>
    /// A dictionary to convert from Healtype to data array.
    /// Notice that only the ones shown on the year panels are taken into account.
    /// </summary>
    public Dictionary<HealthType, float[]> typeDataDictionary;

    public LongTermHealth(List<Health> healths) {
        int[] age = (from health in healths select health.age).ToArray();
        float[] weight = (from health in healths select health.weight).ToArray();
        float[] bmi = (from health in healths select health.BMI).ToArray();
        float[] bodyFatMass = (from health in healths select health.bodyFatMass).ToArray();
        float[] glucose = (from health in healths select health.glucose).ToArray();
        float[] aic = (from health in healths select health.aic).ToArray();
        float[] sbp = (from health in healths select health.sbp).ToArray();
        float[] dbp = (from health in healths select health.dbp).ToArray();
        float[] ldl = (from health in healths select health.ldl).ToArray();
        float[] waistCircumference = (from health in healths select health.waistCircumference).ToArray();
        // Only these metrics are needed for now
        typeDataDictionary = new Dictionary<HealthType, float[]> {
            {HealthType.bmi, bmi},
            {HealthType.bodyFatMass, bodyFatMass},
            {HealthType.aic, aic},
            {HealthType.ldl, ldl},
            {HealthType.sbp, sbp},
        };
    }

    /// <summary>
    /// Gives an overall health point.
    /// </summary>
    /// <returns>The health.</returns>
    /// <param name="index">Index.</param>
    /// <param name="gender">Specifies which set of data to look for.</param>
    public int CalculateHealth(float index, Gender gender) {
        int floorSum = 0;
        int ceilSum = 0;

        int flooredIndex = Mathf.FloorToInt(index);
        int ceiledIndex = Mathf.CeilToInt(index);
        
        foreach (KeyValuePair<HealthType, float[]> entry in typeDataDictionary) {
            floorSum += HealthUtil.CalculatePoint(entry.Key, gender, entry.Value[flooredIndex]);
            ceilSum += HealthUtil.CalculatePoint(entry.Key, gender, entry.Value[ceiledIndex]);
        }
        int floorScore = floorSum / typeDataDictionary.Count;
        int ceilScore = ceilSum / typeDataDictionary.Count;
        return (int)Mathf.Lerp(floorScore, ceilScore, index % 1);
    }

    /// <summary>
    /// Gives a health score for the selected types.
    /// </summary>
    /// <param name="index">Index.</param>
    /// <param name="gender">Specifies which set of data to look for.</param>
    /// <param name="types">Health types to use.</param>
    /// <returns>The health score for the specified index and gender.</returns>
    public int CalculateHealth(float index, Gender gender, params HealthType[] types) {
        int floorSum = 0;
        int ceilSum = 0;

        int flooredIndex = Mathf.FloorToInt(index);
        int ceiledIndex = Mathf.CeilToInt(index);
        
        foreach (HealthType type in types) {
            floorSum += HealthUtil.CalculatePoint(type, gender, typeDataDictionary[type][flooredIndex]);
            ceilSum += HealthUtil.CalculatePoint(type, gender, typeDataDictionary[type][ceiledIndex]);
        }

        return (int)Mathf.Lerp(floorSum, ceilSum, index % 1) / types.Length;
    }
}

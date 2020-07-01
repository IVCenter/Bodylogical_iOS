using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongTermHealth {
    public HealthChoice profileChoice;

    /// <summary>
    /// A dictionary to convert from Healtype to data array.
    /// Notice that only the ones shown on the year panels are taken into account.
    /// </summary>
    public Dictionary<HealthType, float[]> typeDataDictionary;

    public int[] Age { get; }
    public float[] Weight { get; }
    public float[] Bmi { get; }
    public float[] BodyFatMass { get; }
    public float[] Glucose { get; }
    public float[] Aic { get; }
    public float[] Sbp { get; }
    public float[] Dbp { get; }
    public float[] Ldl { get; }
    public float[] WaistCircumference { get; }

    public LongTermHealth(List<Health> healths) {
        Age = (from health in healths select health.age).ToArray();
        Weight = (from health in healths select health.weight).ToArray();
        Bmi = (from health in healths select health.BMI).ToArray();
        BodyFatMass = (from health in healths select health.bodyFatMass).ToArray();
        Glucose = (from health in healths select health.glucose).ToArray();
        Aic = (from health in healths select health.aic).ToArray();
        Sbp = (from health in healths select health.sbp).ToArray();
        Dbp = (from health in healths select health.dbp).ToArray();
        Ldl = (from health in healths select health.ldl).ToArray();
        WaistCircumference = (from health in healths select health.waistCircumference).ToArray();

        typeDataDictionary = new Dictionary<HealthType, float[]>() {
            {HealthType.bmi, Bmi},
            {HealthType.bodyFatMass, BodyFatMass},
            {HealthType.aic, Aic},
            {HealthType.ldl, Ldl},
            {HealthType.sbp, Sbp},
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
        int num = 0;
        foreach (KeyValuePair<HealthType, float[]> entry in typeDataDictionary) {
            num++;
            floorSum += RangeLoader.Instance.CalculatePoint(entry.Key,
                 gender, entry.Value[(int)Mathf.Floor(index)]);
            ceilSum += RangeLoader.Instance.CalculatePoint(entry.Key,
                 gender, entry.Value[(int)Mathf.Floor(index)]);
        }
        int floorScore = floorSum / num;
        int ceilScore = ceilSum / num;
        return (int)Mathf.Lerp(floorScore, ceilScore, index % 1);
    }
}

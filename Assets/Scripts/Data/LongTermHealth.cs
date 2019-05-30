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

    public int[] age;
    public float[] weight;
    public float[] bmi;
    public float[] bodyFatMass;
    public float[] glucose;
    public float[] aic;
    public float[] sbp;
    public float[] dbp;
    public float[] ldl;
    public float[] waistCircumference;

    public LongTermHealth(List<Health> healths) {
        age = (from health in healths select health.age).ToArray();
        weight = (from health in healths select health.weight).ToArray();
        bmi = (from health in healths select health.BMI).ToArray();
        bodyFatMass = (from health in healths select health.bodyFatMass).ToArray();
        glucose = (from health in healths select health.glucose).ToArray();
        aic = (from health in healths select health.aic).ToArray();
        sbp = (from health in healths select health.sbp).ToArray();
        dbp = (from health in healths select health.dbp).ToArray();
        ldl = (from health in healths select health.ldl).ToArray();
        waistCircumference = (from health in healths select health.waistCircumference).ToArray();

        typeDataDictionary = new Dictionary<HealthType, float[]>() {
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
        int num = 0;
        foreach (KeyValuePair<HealthType, float[]> entry in typeDataDictionary) {
            num++;
            floorSum += BiometricContainer.Instance.CalculatePoint(entry.Key,
                 gender, entry.Value[(int)Mathf.Floor(index)]);
            ceilSum += BiometricContainer.Instance.CalculatePoint(entry.Key,
                 gender, entry.Value[(int)Mathf.Floor(index)]);
        }
        int floorScore = floorSum / num;
        int ceilScore = ceilSum / num;
        return (int)Mathf.Lerp(floorScore, ceilScore, index % 1);
    }
}

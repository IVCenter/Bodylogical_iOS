using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongTermHealth : MonoBehaviour {
    public HealthStatus profileStatus;

    /// <summary>
    /// A dictionary to convert from Healtype to data array.
    /// Notice that only the ones shown on the year panels are taken into account.
    /// </summary>
    public Dictionary<HealthType, float[]> typeDataDictionary;

    public int[] age;
    public float[] weight;
    public float[] BMI;
    public float[] bodyFatMass;
    public float[] glucose;
    public float[] aic;
    public float[] sbp;
    public float[] dbp;
    public float[] LDL;
    public float[] waistCircumference;

    void Start() {
        typeDataDictionary = new Dictionary<HealthType, float[]>() {
            {HealthType.bmi, BMI},
            {HealthType.bodyFatMass, bodyFatMass},
            {HealthType.aic, aic},
            {HealthType.ldl, LDL},
            {HealthType.sbp, sbp},
        };
    }

    /// <summary>
    /// Gives an overall health point.
    /// </summary>
    /// <returns>The health.</returns>
    /// <param name="index">Index.</param>
    /// <param name="alt">If set to <c>true</c> alternate.</param>
    public int CalculateHealth(int index, bool alt = false) {
        int sum = 0;
        int num = 0;
        foreach (KeyValuePair<HealthType, float[]> entry in typeDataDictionary) {
            num++;
            sum += BiometricContainer.Instance.StatusRangeDictionary[entry.Key].CalculatePoint(entry.Value[index], false);
        }
        return sum / num;
    }
}

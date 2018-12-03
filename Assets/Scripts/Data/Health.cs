using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class for storing health numbers.
/// </summary>
public class Health : MonoBehaviour {
  public int age;
  public float weight, BMI, bodyFatMass, glucose, aic, sbp, dbp, ldl, waistCircumference;

  // TODO: calculate a score based on the numbers
  public HealthStatus Status {
    get; set;
  }
}

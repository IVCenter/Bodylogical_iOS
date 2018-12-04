using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores range data for one specific health category.
/// </summary>
public class HealthRange : MonoBehaviour {
  public HealthType type;
  public float min, max, warning, upper;
}

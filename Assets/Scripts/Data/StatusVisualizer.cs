using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to generate items around the human model.
/// </summary>
public class StatusVisualizer : MonoBehaviour {
  private GameObject humanModel;
  private Health health;
  public GameObject[] healthObjects;
  public GameObject[] badObjects;

  /// <summary>
  /// Initialize the variables
  /// </summary>
  void Start() {
    humanModel = gameObject;
    health = GetComponent<Health>();
  }
  
  /// <summary>
  /// Places the objects around the human model.
  /// </summary>
  public void Visualize() {
    switch (health.Status) {
      case HealthStatus.bad:
        Instantiate(badObjects[0]);
        // TODO: transform the objects
        break;
      case HealthStatus.good:
        Instantiate(healthObjects[0]);
        break;
    }
  }
}

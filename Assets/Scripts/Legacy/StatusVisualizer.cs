using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to generate items around the human model.
/// </summary>
public class StatusVisualizer : MonoBehaviour {
  public Transform humanModel;
  public Health health;
  public GameObject[] goodObjects;
  public GameObject[] badObjects;

  //private GameObject[] objects;

  /// <summary>
  /// Initialize the variables
  /// </summary>
  void Start() {
    // TODO: currently using public humanmodel and health, can convert to private
    // health = GetComponent<Health>();

    //objects = new GameObject[3];
  }

  /// <summary>
  /// Places the objects around the human model.
  /// </summary>
  public void Visualize() {
    print(transform.childCount);
    // First clear out generated objects
    int i = 0;
    //Array to hold all child obj
    GameObject[] allChildren = new GameObject[transform.childCount - 1];

    //Find all child obj and store to that array
    foreach (Transform child in transform) {
      if (i != 0) {
        allChildren[i - 1] = child.gameObject;
      }
      i += 1;
    }

    //Now destroy them
    foreach (GameObject child in allChildren) {
      DestroyImmediate(child.gameObject);
    }

    GameObject obj;
    // Now generate new ones
    switch (health.status) {
      case HealthStatus.Bad:
        obj = Instantiate(badObjects[0]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0.2f, 0, 0);

        obj = Instantiate(badObjects[1]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(-0.2f, 0, 0);

        obj = Instantiate(badObjects[2]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0, 0, 0.2f);
        break;
      case HealthStatus.Good:
        obj = Instantiate(goodObjects[0]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0.2f, 0, 0);

        obj = Instantiate(goodObjects[1]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(-0.2f, 0, 0);

        obj = Instantiate(goodObjects[2]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0, 0, 0.2f);
        break;
      case HealthStatus.Intermediate:
        obj = Instantiate(badObjects[3]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0.2f, 0, 0);

        obj = Instantiate(badObjects[4]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(-0.2f, 0, 0);

        obj = Instantiate(goodObjects[1]);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0, 0, 0.2f);
        break;
    }
  }

  #region Test
  /// <summary>
  /// FOR TESTING ONLY.
  /// </summary>
  [Header("Test")]
  public HealthStatus status;
  void OnValidate() {
    health.status = status;
    // In the editor Destroy() cannot be called, so new objects will stack over old ones.
    // If you want to see how the script generates objects, uncomment the line below,
    // but remember to remove generated objects.
    //Visualize();
  }
  #endregion
}

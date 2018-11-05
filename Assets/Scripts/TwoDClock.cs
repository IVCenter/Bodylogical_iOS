using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDClock : MonoBehaviour {
  public RectTransform pointer;
  public int sleepTime;

  public void Rotate(float deg) {
    pointer.Rotate(new Vector3(0, 0, -deg));
  }

  public void SetDegree(float deg) {
    pointer.eulerAngles = new Vector3(0, 0, -deg);
  }

  void OnValidate() {
    SetDegree(sleepTime * 30);
  }
}

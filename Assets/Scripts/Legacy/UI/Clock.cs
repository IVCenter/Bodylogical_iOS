using System;
using UnityEngine;

public class Clock : MonoBehaviour {

  public Transform head;

	// Update is called once per frame
	void Update () {
    head.localRotation = Quaternion.Euler(-90f, 0f, DateTime.Now.Second * 6f);
	}
}

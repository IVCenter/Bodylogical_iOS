using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour {

    private GameObject Target;

    private void Start()
    {
        Target = Camera.main.gameObject;
    }

    // LateUpdate is called after camera location has been recalculated
    void LateUpdate () {
        transform.LookAt(Target.transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90,0,0));
	}
}

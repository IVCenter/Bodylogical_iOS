using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideColliders : MonoBehaviour {

    /// <summary>
    /// Make Collider collide with Gaze input
    /// </summary>
    private void Update()
    {
        gameObject.layer = 0;
    }
    
    /// <summary>
    /// Make Collider invisible to user so they can't select it accidentally
    /// </summary>
    void LateUpdate () {
        gameObject.layer = 2;
	}

    private void OnPreRender()
    {
        gameObject.layer = 2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Some animations would shift the model's position, some wouldn't. This would
/// cause trouble in determining model movement speed.
/// This script pins the models to its local origin, so that all animations can
/// be treated as if they do not shift the models.
/// </summary>
public class PinModel : MonoBehaviour {
    void Update() {
        transform.localPosition = new Vector3(0, 0, 0);
    }
}

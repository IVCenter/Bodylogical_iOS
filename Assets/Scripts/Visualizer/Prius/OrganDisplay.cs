using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrganDisplay : MonoBehaviour {
    public abstract void DisplayOrgan(int score = 0, HealthStatus status = HealthStatus.Bad);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelItem : MonoBehaviour {
    public abstract void SetValue(float value, int index = 0);
    public abstract void SetText();
}

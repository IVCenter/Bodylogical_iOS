using UnityEngine;

/// <summary>
/// A component with a value and a text status, to be displayed on a 2D canvas.
/// </summary>
public abstract class PanelItem : MonoBehaviour {
    public abstract void SetValue(int index, float value);
    public abstract void SetText();
}

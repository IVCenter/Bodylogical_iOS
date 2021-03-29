using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public VisualizeIcon Icon { get; private set; }

    private void Start() {
        Icon = GetComponentInChildren<VisualizeIcon>(true);
    }

    public void Reset() {
        panel.Reset();
        Icon.ResetIcon();
    }
}
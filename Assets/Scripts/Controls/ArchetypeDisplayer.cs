using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public HeaderText Header { get; }
    public VisualizeIcon Icon { get; }

    public ArchetypeDisplayer(Archetype archetypeData, Transform parent)
        : base(ArchetypeManager.Instance.displayerPrefab, archetypeData, parent) {
        Header = Model.GetComponentInChildren<HeaderText>();
        Header.SetInfo(archetypeData);

        Icon = Model.GetComponentInChildren<VisualizeIcon>(true);
    }
}
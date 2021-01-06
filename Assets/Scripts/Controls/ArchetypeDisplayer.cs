using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public InfoPanel Info { get; }
    public VisualizeIcon Icon { get; }

    public ArchetypeDisplayer(Archetype archetypeData, Transform parent) : base(archetypeData, parent) {
        Info = Model.GetComponentInChildren<InfoPanel>();
        Info.Initialize(archetypeData);

        Icon = Model.GetComponentInChildren<VisualizeIcon>(true);
    }
}
using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public DetailPanel Panel { get; }
    public InfoPanel Info { get; }
    public VisualizeIcon Icon { get; }

    public ArchetypeDisplayer(Archetype archetypeData, Transform parent)
        : base(ArchetypeManager.Instance.displayerPrefab, archetypeData, parent) {
        Panel = Model.GetComponentInChildren<DetailPanel>(true);
        Panel.Initialize(this);
        
        Info = Model.GetComponentInChildren<InfoPanel>();
        Info.Initialize(archetypeData);

        Icon = Model.GetComponentInChildren<VisualizeIcon>(true);
    }
}
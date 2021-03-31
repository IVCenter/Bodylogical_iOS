/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public VisualizeIcon icon;

    public void Reset() {
        panel.Reset();
        icon.ResetIcon();
    }
}
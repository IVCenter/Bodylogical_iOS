/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public HeaderText Header { get; private set; }
    public VisualizeIcon Icon { get; private set; }

    private void Start() {
        Header = model.GetComponentInChildren<HeaderText>();
        Icon = model.GetComponentInChildren<VisualizeIcon>(true);
    }

    public void Initialize() {
        Header.SetInfo(ArchetypeData);
    }

    public void Reset() {
        Header.SetInfo(ArchetypeData);
        panel.Reset();
        Icon.ResetIcon();
    }
}
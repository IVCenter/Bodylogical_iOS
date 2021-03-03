public class ArchetypeInteract : Interactable {
    public ArchetypeDisplayer Displayer { get; set; }
    
    public override void OnTouchUp() {
        if (AppStateManager.Instance.CurrState == AppState.PickArchetype) {
            ArchetypeManager.Instance.Selected = Displayer;
        }
    }
}

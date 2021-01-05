public class ArchetypeInteract : Interactable {
    public ArchetypeBase Base { get; set; }
    
    public override void OnTouchUp() {
        if (AppStateManager.Instance.CurrState == AppState.PickArchetype) {
            ArchetypeManager.Instance.Selected = Base;
        }
    }
}

public class ArchetypeInteract : Interactable {
    public bool IsSelected { get; set; }

    public override void OnTouchUp() {
        if (AppStateManager.Instance.CurrState == AppState.PickArchetype) {
            IsSelected = true;
        }
    }
}

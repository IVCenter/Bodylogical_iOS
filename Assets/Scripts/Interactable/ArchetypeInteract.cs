using UnityEngine;

public class ArchetypeInteract : Interactable {
    [HideInInspector]
    public bool isSelected;

    public override void OnTouchUp() {
        if (AppStateManager.Instance.currState == AppState.PickArchetype) {
            DebugText.Instance.Log("A human is selected");
            isSelected = true;
        }
    }
}

using UnityEngine;

public class HumanInteract : Interactable {
    [HideInInspector]
    public bool isSelected;

    public override void OnTouchUp() {
        if (MasterManager.Instance.currPhase == GamePhase.PickArchetype) {
            DebugText.Instance.Log("A human is selected");
            isSelected = true;
        }
    }
}

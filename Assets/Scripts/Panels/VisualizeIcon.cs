using UnityEngine;

public class VisualizeIcon : MonoBehaviour {
    private bool clicked;
    
    public void SetActive(bool on) {
        gameObject.SetActive(on);
    }
    
    public void OnClick() {
        if (!clicked) {
            StageManager.Instance.StartVisualizations();
            clicked = true;
        }
    }

    public void ResetIcon() {
        clicked = false;
    }
}

using UnityEngine;

public class VisualizeIcon : MonoBehaviour {
    public void SetActive(bool on) {
        gameObject.SetActive(on);
    }
    
    public void OnClick() {
        StageManager.Instance.StartVisualizations();
    }
}

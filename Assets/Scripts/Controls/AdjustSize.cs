using UnityEngine;

public class AdjustSize : MonoBehaviour {
    private void Start() {
        float scale = DeviceManager.Instance.Constants.stageSize;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
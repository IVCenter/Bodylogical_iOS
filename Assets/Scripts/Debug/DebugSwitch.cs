using UnityEngine;

// Enable/Disable components for debugging.
public class DebugSwitch : MonoBehaviour {
    public GameObject[] deviceObjects;
    public GameObject[] debugObjects;

    public Camera deviceCamera, debugCamera;
    public Canvas tutorialCanvas;

    private void Awake() {
        foreach (GameObject obj in deviceObjects) {
#if UNITY_EDITOR
            obj.SetActive(false);
#else
            obj.SetActive(true);
#endif
        }

        foreach (GameObject obj in debugObjects) {
#if UNITY_EDITOR
            obj.SetActive(true);
#else
            obj.SetActive(false);
#endif
        }

        // Camera settings
#if UNITY_EDITOR
        debugCamera.tag = "MainCamera";
        tutorialCanvas.worldCamera = debugCamera;
#else
        deviceCamera.tag = "MainCamera";
        tutorialCanvas.worldCamera = deviceCamera;
#endif
    }
}

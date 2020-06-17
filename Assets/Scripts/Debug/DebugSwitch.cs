using UnityEngine;

// Enable/Disable components for debugging.
public class DebugSwitch : MonoBehaviour {
    public GameObject[] deviceObjects;
    public GameObject[] debugObjects;

    public Camera deviceCamera, debugCamera;
    public Canvas tutorialCanvas;

    private void Awake() {
        foreach (GameObject obj in deviceObjects) {
            if (Application.isEditor) {
                obj.SetActive(false);
            } else {
                obj.SetActive(true);
            }
        }

        foreach (GameObject obj in debugObjects) {
            if (Application.isEditor) {
                obj.SetActive(true);
            } else {
                obj.SetActive(false);
            }
        }

        // Camera settings
        if (Application.isEditor) {
            debugCamera.tag = "MainCamera";
            tutorialCanvas.worldCamera = debugCamera;
        } else {
            deviceCamera.tag = "MainCamera";
            tutorialCanvas.worldCamera = deviceCamera;
        }
    }
}

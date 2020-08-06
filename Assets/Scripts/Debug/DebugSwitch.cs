using UnityEngine;

// Enable/Disable components for debugging.
public class DebugSwitch : MonoBehaviour {
    public GameObject[] deviceObjects;
    public GameObject[] debugObjects;

    public Camera deviceCamera, debugCamera;
    public Canvas tutorialCanvas;

    private void Awake() {
        foreach (GameObject obj in deviceObjects) {
            obj.SetActive(!Application.isEditor);
        }

        foreach (GameObject obj in debugObjects) {
            obj.SetActive(Application.isEditor);
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

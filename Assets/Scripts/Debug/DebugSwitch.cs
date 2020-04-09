using UnityEngine;
using UnityEngine.UI;

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

#if UNITY_EDITOR
        tutorialCanvas.worldCamera = debugCamera;
#else
        tutorialCanvas.worldCamera = deviceCamera;
#endif
    }
}

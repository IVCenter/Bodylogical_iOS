using UnityEngine;

// Enable/Disable components for debugging.
public class DebugSwitch : MonoBehaviour {
    public GameObject[] deviceObjects;
    public GameObject[] debugObjects;

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
    }
}

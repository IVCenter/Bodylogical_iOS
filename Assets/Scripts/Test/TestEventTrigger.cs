using UnityEngine;

public class TestEventTrigger : MonoBehaviour {
    public void Log(string str) {
        Debug.LogError("Interacted " + str);
    }
}

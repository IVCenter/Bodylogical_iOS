using UnityEngine;
using UnityEngine.UI;

public class TestTextSize : MonoBehaviour {
    [SerializeField] private Text text;
    [SerializeField] private bool trigger;

    private void OnValidate() {
        Debug.Log($"Height: {text.preferredHeight}, Width: {text.preferredWidth}");
    }
}

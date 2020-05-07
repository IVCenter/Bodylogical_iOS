using UnityEngine;

/// <summary>
/// Takes in a list of objects and toggle their visibility.
/// </summary>
public class Toggler : MonoBehaviour {
    [SerializeField] private GameObject[] objects;

    public void Toggle() {
        foreach (GameObject obj in objects) {
            obj.SetActive(!obj.activeSelf);
        }
    }
}

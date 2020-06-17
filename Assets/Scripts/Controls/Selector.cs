using UnityEngine;

/// <summary>
/// Takes in a list of objects and selects a specific one.
/// </summary>
public class Selector : MonoBehaviour {
    [SerializeField] private GameObject[] objects;

    public void Select(int index) {
        for (int i = 0; i < objects.Length; i++) {
            objects[i].SetActive(i == index);
        }
    }
}
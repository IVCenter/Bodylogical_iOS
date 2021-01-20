using UnityEngine;

public class Billboard : MonoBehaviour {
    [SerializeField] private bool reverseDirection = true;
    private static Transform cam;

    private Vector3 Target => reverseDirection ? transform.position * 2 - cam.position : cam.position;

    private void Start() {
        if (cam == null) {
            cam = Camera.main.transform;
        }
    }

    private void Update() {
        transform.LookAt(Target);
    }
}
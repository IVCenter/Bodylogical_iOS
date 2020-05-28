using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public int TouchCount {
        get {
            if (Application.isEditor) {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                    return 1;
                }
                return 0;
            } else {
                return Input.touchCount;
            }
        }
    }

    public int TapCount {
        get {
            if (Application.isEditor) {
                if (Input.GetMouseButton(0)) {
                    return 1;
                }
                if (Input.GetMouseButton(1)) {
                    return 2;
                }
                return 0;
            } else {
                return Input.GetTouch(0).tapCount;
            }
        }
    }

    public Vector2 ScreenPos {
        get {
            if (Application.isEditor) {
                return Input.mousePosition;
            } else {
                return Input.GetTouch(0).position;
            }
        }
    }

    public Vector3 WorldPos => GetWorldSpace(ScreenPos);

    public Vector2 CenterPos => new Vector2(Screen.width / 2, Screen.height / 2);

    private Vector3 GetWorldSpace(Vector2 coords) {
        Camera cam = Camera.main;
        return cam.ScreenToWorldPoint(new Vector3(coords.x, coords.y, cam.nearClipPlane));
    }
}

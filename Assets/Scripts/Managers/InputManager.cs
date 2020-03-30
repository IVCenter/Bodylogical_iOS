using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    /// <summary>
    /// If a 2D overlay menu is opened, we need to temporarily disable the debug camera.
    /// Also, if we want to switch the old "cursor" interaction, the cursor should
    /// be disabled in a 2D menu.
    /// At first, the user is prompted to choose language, so the menu is opened
    /// (thus it is default to true)
    /// </summary>
    [HideInInspector] public bool menuOpened = true;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public int TouchCount {
        get {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                return 1;
            }
            return 0;
#else
            return Input.touchCount;
#endif
        }
    }

    public int TapCount {
        get {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0)) {
                return 1;
            }
            if (Input.GetMouseButton(1)) {
                return 2;
            }
            return 0;
#else
            return Input.GetTouch(0).tapCount;
#endif
        }
    }

    public Vector2 ScreenPos {
        get {
#if UNITY_EDITOR
            return Input.mousePosition;
#else
            return Input.GetTouch(0).position;
#endif
        }
    }

    public Vector3 WorldPos => GetWorldSpace(ScreenPos);

    public Vector2 CenterPos => new Vector2(Screen.width / 2, Screen.height / 2);

    private Vector3 GetWorldSpace(Vector2 coords) {
        Camera cam = Camera.main;
        return cam.ScreenToWorldPoint(new Vector3(coords.x, coords.y, cam.nearClipPlane));
    }
}

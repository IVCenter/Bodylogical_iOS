using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    public Cursor cursor;

    [HideInInspector]
    /// <summary>
    /// At first, the user is prompted to choose language, so a 2d menu is opened.
    /// </summary>
    public bool menuOpened = true;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public int TouchCount {
        get {
            return menuOpened ? 0 : Input.touchCount;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    public Cursor cursor;

    [HideInInspector]
    /// <summary>
    /// If a 2D overlay menu is opened, we need to temporarily disable 3D cursor
    /// interaction.
    /// At first, the user is prompted to choose language, so a 2d menu is opened
    /// (thus it is default to true)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {
    public static CursorManager Instance { get; private set; }

    public Cursor cursor;

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }
    }
}

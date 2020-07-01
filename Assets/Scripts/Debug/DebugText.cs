using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {
    public static DebugText Instance { get; private set; }

    public Text debugText;
    public int maxLine = 10;
    private int lineCount = 0;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void Log(string content) {
        if (lineCount + 1 > maxLine) {
            debugText.text = "";
            lineCount = 0;
        }

        debugText.text += "\n" + content;
        lineCount += 1;
    }
}

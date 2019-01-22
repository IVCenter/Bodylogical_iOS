using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    public static DebugText Instance;

    public Text debugText;
    public int max_line = 10;
    private int line_count = 0;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void Log(string content) {
        //debugText = GameObject.Find("Canvas").GetComponentInChildren<Text>();

        if (line_count + 1 > max_line) {
            debugText.text = "";
            line_count = 0;
        }

        debugText.text += "\n";
        debugText.text += content;
        line_count += 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {
    public string key;

    private string[] param;

    public void SetText(string str, params string[] args) {
        // TODO: when update language, need to keep original params as well.
        GetComponent<Text>().text = string.Format(str, args);
    }
}

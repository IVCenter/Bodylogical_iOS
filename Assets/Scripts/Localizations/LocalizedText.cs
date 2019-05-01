using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {
    public string key = "";

    public string[] param;

    /// <summary>
    /// There are two situations this method will be called:
    /// 1. When the program manually sets the key and arg
    /// 2. When the user changes the language or the program starts up. In this case,
    /// the script needs to remember the key and the param so that it can update without
    /// any outside input.
    /// </summary>
    /// <param name="str">New key. Takes the form of [DictionaryName].[id]. If null, use key.</param>
    /// <param name="args">Arguments. If empty, use param.</param>
    public void SetText(string str, params string[] args) {
        if (str != null) {
            key = str;
        }
        if (args.Length != 0) {
            param = args;
        }

        if (!string.IsNullOrEmpty(key)) {
            string[] keys = key.Split('.');
            string original = GetDict(keys[0])[keys[1]];
            if (param.Length != 0) {
                GetComponent<Text>().text = string.Format(original, param);
            } else {
                GetComponent<Text>().text = original;
            }
        }
    }

    /// <summary>
    /// Gets the dictionary for localization.
    /// </summary>
    /// <returns>The dictionary</returns>
    /// <param name="str">dictionary name.</param>
    private Dictionary<string, string> GetDict(string str) {
        switch (str) {
            case "General":
                return LocalizationManager.Instance.currLocalization.general;
            case "Buttons":
                return LocalizationManager.Instance.currLocalization.buttons;
            case "Legends":
                return LocalizationManager.Instance.currLocalization.legends;
            case "Archetypes":
                return LocalizationManager.Instance.currLocalization.archetypes;
            case "Instructions":
                return LocalizationManager.Instance.currLocalization.instructions;
            default:
                throw new System.ArgumentException("Dict name does not exist");
        }
    }
}
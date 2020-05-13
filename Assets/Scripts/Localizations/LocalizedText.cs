using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {
    public string key = "";
    public LocalizedParam[] pars;

    /// <summary>
    /// There are two situations this method will be called:
    /// 1. When the program manually sets the key and arg
    /// 2. When the user changes the language or the program starts up. In this case,
    /// the script needs to remember the key and the param so that it can update without
    /// any outside input.
    /// </summary>
    /// <param name="str">New key. Takes the form of [DictionaryName].[id]. If null, use key.</param>
    /// <param name="args">Arguments. If empty, use param.</param>
    public void SetText(string str, params LocalizedParam[] args) {
        if (str != null) {
            key = str;
        }
        if (args.Length != 0) {
            pars = args;
        }

        GetComponent<Text>().text = LocalizationManager.Instance.FormatString(key, pars);
    }

    public void Clear() {
        key = "";
        pars = new LocalizedParam[0];
        GetComponent<Text>().text = "";
    }
}
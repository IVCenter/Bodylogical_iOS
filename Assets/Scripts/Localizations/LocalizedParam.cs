using UnityEngine.Serialization;

[System.Serializable]
public class LocalizedParam {
    public string str;
    public bool isKey;

    public LocalizedParam(string s = "", bool b = false) {
        str = s;
        isKey = b;
    }

    public LocalizedParam(object obj, bool b = false) {
        str = obj.ToString();
        isKey = b;
    }

    public override string ToString() {
        return isKey ? LocalizationManager.Instance.GetText(str) : str;
    }
}

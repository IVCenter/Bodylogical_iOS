using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private Language language = Language.en_US;
    private LocalizedText[] texts;
    private Localization currLocalization;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        texts = Resources.FindObjectsOfTypeAll<LocalizedText>();

        TextAsset locale = Resources.Load<TextAsset>("Localizations/locale-en_US");
        currLocalization = new Localization(language, locale.text);
        UpdateTexts();
    }

    private void UpdateTexts() {
        foreach (LocalizedText text in texts) {
            text.SetText(null);
        }
    }

    public void ChangeLanguage(Language lang) {
        if (language == lang) {
            return;
        }
        
        // There will be dynamically generated assets, so need to refresh the component array.
        texts = Resources.FindObjectsOfTypeAll<LocalizedText>();
        TextAsset locale = Resources.Load<TextAsset>($"Localizations/locale-{language}");
        currLocalization = new Localization(language, locale.text);
        UpdateTexts();
    }

    public string GetText(string key) {
        string[] keys = key.Split('.');
        try {
            return GetDict(keys[0])[keys[1]];
        } catch (KeyNotFoundException) {
            Debug.LogError($"Cannot find key {keys[1]} in dictionary {keys[0]}");
            return "ERROR";
        }
    }

    /// <summary>
    /// Gets the dictionary for localization.
    /// </summary>
    /// <returns>The dictionary</returns>
    /// <param name="str">dictionary name.</param>
    private Dictionary<string, string> GetDict(string str) {
        return typeof(Localization).GetProperty(str)?.GetValue(currLocalization) as Dictionary<string, string>;
    }

    public string FormatString(string key, params LocalizedParam[] args) {
        if (!string.IsNullOrEmpty(key)) {
            string original = GetText(key);
            if (args.Length != 0) {
                return string.Format(original, args);
            }

            return original;
        }

        return "";
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {
    public static LocalizationManager Instance { get; private set; }

    private Language language = Language.en_US;

    private LocalizedText[] texts;

    public Localization currLocalization;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        texts = Resources.FindObjectsOfTypeAll(typeof(LocalizedText)) as LocalizedText[];

        TextAsset locale = Resources.Load<TextAsset>("Localizations/locale-en_US");
        currLocalization = new Localization(language, locale.text);
        UpdateTexts();
    }

    public void UpdateTexts() {
        foreach (LocalizedText text in texts) {
            text.SetText(null);
        }
    }

    public void ChangeLanguage(int lang) {
        if (language != (Language)lang) {
            language = (Language)lang;
            // There will be dynamically generated assets, so need to refresh the component array.
            texts = Resources.FindObjectsOfTypeAll(typeof(LocalizedText)) as LocalizedText[];
            TextAsset locale = Resources.Load<TextAsset>("Localizations/locale-" + language.ToString());
            currLocalization = new Localization(language, locale.text);
            UpdateTexts();
        }
    }

    public string GetText(string key) {
        string[] keys = key.Split('.');
        return GetDict(keys[0])[keys[1]];
    }

    /// <summary>
    /// Gets the dictionary for localization.
    /// </summary>
    /// <returns>The dictionary</returns>
    /// <param name="str">dictionary name.</param>
    private Dictionary<string, string> GetDict(string str) {
        switch (str) {
            case "General":
                return currLocalization.general;
            case "Buttons":
                return currLocalization.buttons;
            case "Legends":
                return currLocalization.legends;
            case "Archetypes":
                return currLocalization.archetypes;
            case "Instructions":
                return currLocalization.instructions;
            default:
                throw new System.ArgumentException("Dict name does not exist");
        }
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

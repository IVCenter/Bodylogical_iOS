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
            TextAsset locale = Resources.Load<TextAsset>("Localizations/locale-" + language.ToString());
            currLocalization = new Localization(language, locale.text);
            UpdateTexts();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {
    public static LocalizationManager Instance { get; private set; }

    private Language language = Language.en_US;
    public Language Lang {
        get {
            return language;
        }
        set {
            if (language != value) {
                language = value;
                UpdateTexts();
            }
        }
    }

    private LocalizedText[] texts;

    private Localization currLocalization;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        texts = Resources.FindObjectsOfTypeAll(typeof(LocalizedText)) as LocalizedText[];
        currLocalization = new Localization(language, Path.Combine(Application.dataPath, string.Format("Localizations/locale-{0}.xml", language.ToString())));
    }

    public void UpdateTexts() {
        foreach (LocalizedText text in texts) {
            text.SetText("");
        }
    }
}

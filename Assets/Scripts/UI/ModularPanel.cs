using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For a detail panel with sections that can be added or subtracted.
/// </summary>
public class ModularPanel : MonoBehaviour {
    public RectTransform[] sections;

    public void Hide(int index) {
        sections[index].gameObject.SetActive(false);
    }

    public void Show(int index) {
        sections[index].gameObject.SetActive(true);
    }

    public void HideAll() {
        for (int i = 0; i < sections.Length; i++) {
            sections[i].gameObject.SetActive(false);
        }
    }

    public void ShowAll() {
        for (int i = 0; i < sections.Length; i++) {
            sections[i].gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For a detail panel with sections that can be added or subtracted.
/// </summary>
public class ModularPanel : MonoBehaviour {
  public RectTransform[] sections;
  public int topCoord;

  public void Hide(int index) {
    bool active = sections[index].gameObject.activeSelf;
    sections[index].gameObject.SetActive(false);
    // If previously active, rearrange
    if (active) {
      for (int i = index + 1; i < sections.Length; i++) {
        sections[i].anchoredPosition = new Vector2(sections[i].anchoredPosition.x,
          sections[i].anchoredPosition.y + 100);
      }
    }
  }

  public void Show(int index) {
    bool active = sections[index].gameObject.activeSelf;
    sections[index].gameObject.SetActive(true);
    // If previously hidden, rearrange
    if (!active) {
      for (int i = index + 1; i < sections.Length; i++) {
        sections[i].anchoredPosition = new Vector2(sections[i].anchoredPosition.x,
          sections[i].anchoredPosition.y - 100);
      }
    }
  }

  public void HideAll() {
    for (int i = 0; i < sections.Length; i++) {
      sections[i].gameObject.SetActive(false);
      sections[i].anchoredPosition = new Vector2(sections[i].anchoredPosition.x,
        topCoord);
    }
  }

  public void ShowAll() {
    for (int i = 0; i < sections.Length; i++) {
      sections[i].gameObject.SetActive(true);
      sections[i].anchoredPosition = new Vector2(sections[i].anchoredPosition.x,
        topCoord - i * 100);
    }
  }

  /// <summary>
  /// FOR TESTING PURPOSES ONLY.
  /// </summary>
  [Header("Testing")]
  public int index;
  public bool show;
  public bool showAllToggle, hideAllToggle;
  void OnValidate() {
    if (showAllToggle) {
      ShowAll();
    } else if (hideAllToggle) {
      HideAll();
    } else if (show) {
      Show(index);
    } else {
      Hide(index);
    }
  }
}

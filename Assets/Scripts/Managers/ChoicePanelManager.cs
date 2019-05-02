using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO: set lifestyle of choice panels.
/// </summary>
public class ChoicePanelManager : MonoBehaviour {
    public static ChoicePanelManager Instance { get; private set; }

    public GameObject choicePanels;
    
    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void ToggleChoicePanels(bool on) {
        choicePanels.SetActive(on);
    }

    public void ToggleChoicePanels() {
        choicePanels.SetActive(!choicePanels.activeSelf);
    }
}

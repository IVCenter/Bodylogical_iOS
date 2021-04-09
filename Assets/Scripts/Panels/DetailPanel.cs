using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the panels that display an avatar's basic data.
/// </summary>
public class DetailPanel : MonoBehaviour {
    [SerializeField] private PanelItem weight, glucose, hba1c, bloodPressure;
    [SerializeField] private Image[] panels;
    [SerializeField] private ColorLibrary colorLibrary;

    [SerializeField] private LocalizedText text;

    // Only used for displayers.
    [SerializeField] private ArchetypeModel model;

    [SerializeField] private float cycleInterval = 0.2f;
    [SerializeField] private float pauseInterval = 5f;

    private readonly bool[] panelOpened = new bool[4];
    private bool lockIcon;

    private ExpandableWindow[] windows;

    private ExpandableWindow[] Windows => windows ?? (windows = GetComponentsInChildren<ExpandableWindow>(true));

    public bool AllClicked => panelOpened.All(opened => opened);

    private LongTermHealth longTermHealth;
    private IEnumerator coroutine;

    /// <summary>
    /// Updates the items on the detail panels.
    /// </summary>
    public void SetValues(LongTermHealth health, bool setColor = false) {
        longTermHealth = health;

        if (setColor) {
            Color c = colorLibrary.ChoiceColorDict[health.choice];

            foreach (Image panel in panels) {
                float alpha = panel.color.a;
                c.a = alpha;
                panel.color = c;
            }
        }
    }

    public void CycleData(bool on) {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            text.gameObject.SetActive(false);
        }

        if (on) {
            text.gameObject.SetActive(true);
            StartCoroutine(coroutine = Cycle());
        }
    }

    private IEnumerator Cycle() {
        while (true) {
            for (float i = 0; i < longTermHealth.Count - 1; i += cycleInterval) {
                UpdateStats(i);
                yield return new WaitForSeconds(cycleInterval);
            }

            yield return new WaitForSeconds(pauseInterval);
        }
    }

    public void UpdateStats(float i) {
        Health h = Health.Interpolate(longTermHealth[Mathf.FloorToInt(i)], longTermHealth[Mathf.CeilToInt(i)],
            i % 1);

        text.SetText("Legends.Date", new LocalizedParam(h.date.Year), new LocalizedParam(h.date.Month));

        weight.SetValue(0, h[HealthType.weight]);
        weight.SetValue(1, h[HealthType.bmi]);

        glucose.SetValue(0, h[HealthType.glucose]);
        hba1c.SetValue(0, h[HealthType.aic]);

        bloodPressure.SetValue(0, h[HealthType.sbp]);
        bloodPressure.SetValue(1, h[HealthType.dbp]);
    }

    public void Toggle(bool on) {
        gameObject.SetActive(on);
        if (on) {
            foreach (ExpandableWindow window in Windows) {
                window.Pulse();
            }
        }
    }

    /// <summary>
    /// Monitors the number of expandable panels that has been opened.
    /// This method will only be called on the panel attached to an ArchetypeDisplayer.
    /// </summary>
    public void OnPanelClick(int index) {
        if (lockIcon) {
            return;
        }

        panelOpened[index] = true;


        if (AllClicked) {
            // All four panels are opened, show icon
            lockIcon = true;
            ((ArchetypeDisplayer) model).icon.SetActive(true);
        }
    }

    public void Reset() {
        Toggle(false);
        for (int i = 0; i < 4; i++) {
            panelOpened[i] = false;
        }

        lockIcon = false;
        foreach (ExpandableWindow window in Windows) {
            window.Reset();
        }
    }
}
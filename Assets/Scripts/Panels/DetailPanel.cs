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

    private Health currHealth;

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
        currHealth = Health.Interpolate(longTermHealth[Mathf.FloorToInt(i)], longTermHealth[Mathf.CeilToInt(i)],
            i % 1);

        UpdateStats();
    }

    /// <summary>
    /// This method only updates the data. It is only called upon unit (SI/Imperial) changes..
    /// </summary>
    public void UpdateStats() {
        if (currHealth == null) {
            return;
        }
        
        text.SetText("Legends.Date", new LocalizedParam(currHealth.date.Year),
            new LocalizedParam(currHealth.date.Month));

        // Weight could be in kg or lb
        weight.SetValue(0, UnitManager.Instance.GetWeight(currHealth[HealthType.weight]));
        weight.SetValue(1, currHealth[HealthType.bmi]);

        glucose.SetValue(0, currHealth[HealthType.glucose]);
        hba1c.SetValue(0, currHealth[HealthType.aic]);

        bloodPressure.SetValue(0, currHealth[HealthType.sbp]);
        bloodPressure.SetValue(1, currHealth[HealthType.dbp]);
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
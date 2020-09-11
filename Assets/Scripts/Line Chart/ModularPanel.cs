using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A composite panel consisting of individual sections.
/// </summary>
public class ModularPanel : MonoBehaviour {
    public GameObject[] sections;
    [SerializeField] private float animationTime = 2.0f;

    private HealthType? highlighted;
    private static readonly Dictionary<HealthType, int> typeSectionDictionary = new Dictionary<HealthType, int> {
        { HealthType.overall, 0 },
        { HealthType.bodyFatMass, 1 },
        { HealthType.bmi, 2 },
        { HealthType.aic, 3 },
        { HealthType.ldl, 4 },
        { HealthType.sbp, 5 }
    };
    private readonly float rightBound = 1100f;
    private RectTransform[] sectionRT;
    private float[] sectionY;

    private void Start() {
        sectionRT = new RectTransform[sections.Length];
        sectionY = new float[sections.Length];
        for (int i = 0; i < sections.Length; i++) {
            sectionRT[i] = sections[i].GetComponent<RectTransform>();
            sectionY[i] = sectionRT[i].anchoredPosition.y;
        }
    }

    /// <summary>
    /// Sets the min, max, warning and upper bounds for the sliders.
    /// </summary>
    public void SetBounds() {
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            LinearIndicatorSlideBarManager manager =
                sections[pair.Value].GetComponent<IndicatorPanelItem>().slideBarManager
                as LinearIndicatorSlideBarManager;

            Gender gender = ArchetypeManager.Instance.Selected.ArchetypeData.gender;

            manager.min = RangeLoader.Instance.GetRange(pair.Key, gender).min;
            manager.max = RangeLoader.Instance.GetRange(pair.Key, gender).max;

            // for body fat, males and females have differnt warning and upper values.
            float warning = RangeLoader.Instance.GetRange(pair.Key, gender).warning;
            float upper = RangeLoader.Instance.GetRange(pair.Key, gender).upper;
            manager.warning = warning;
            manager.upper = upper;

            manager.SetBackground();
        }
    }

    /// <summary>
    /// Sets the values.
    /// </summary>
    /// <param name="index">Index, a.k.a. year value.</param>
    public void SetValues(int index, HealthChoice choice) {
        Gender gender = ArchetypeManager.Instance.Selected.ArchetypeData.gender;
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            IndicatorPanelItem item = sections[pair.Value].GetComponent<IndicatorPanelItem>();
            if (pair.Key != HealthType.overall) {
                item.SetValue(HealthLoader.Instance.ChoiceDataDictionary[choice].typeDataDictionary[pair.Key][index]);
            } else {
                item.SetValue(HealthLoader.Instance.ChoiceDataDictionary[choice].CalculateHealth(index, gender));
            }
        }
    }

    /// <summary>
    /// Hide/Show the specific section.
    /// </summary>
    /// <returns>true if the section has been shown, else false.</returns>
    /// <param name="index">Index of the section.</param>
    public void Toggle(int index, bool on) {
        sections[index].gameObject.SetActive(on);
    }

    /// <summary>
    /// Hide/Show the section's background.
    /// </summary>
    /// <param name="on">If set to <c>true</c> set them to transparent.</param>
    public void ToggleAllBackground(bool on) {
        foreach (GameObject section in sections) {
            section.GetComponent<Image>().enabled = !on;
        }
    }

    /// <summary>
    /// Hide/Show the bar's normal, warning and upper colors.
    /// </summary>
    /// <param name="on">If set to <c>true</c> on.</param>
    public void ToggleAllBars(bool on) {
        foreach (GameObject section in sections) {
            LinearIndicatorSlideBarManager manager =
                section.GetComponent<IndicatorPanelItem>().slideBarManager
                as LinearIndicatorSlideBarManager;
            manager.background.ToggleBackground(!on);
        }
    }

    /// <summary>
    /// Toggle section color between light/dark.
    /// </summary>
    /// <param name="on">If set to <c>true</c> turn to dark.</param>
    public void ToggleColor(bool on) {
        foreach (GameObject section in sections) {
            LinearIndicatorSlideBarManager manager =
                section.GetComponent<IndicatorPanelItem>().slideBarManager
                as LinearIndicatorSlideBarManager;
            manager.background.ToggleBackgroundColor(on);
        }
    }

    /// <summary>
    /// Pulls a section to the right of the panel, or restore it to the original
    /// position.
    /// </summary>
    /// <param name="index">Index of the panel.</param>
    /// <param name="on">If set to <c>true</c> pull to the right.
    /// If false, restore to the left.</param>
    public IEnumerator PullSection(HealthType type) {
        float timePassed = 0;

        float[] endCoord = new float[sections.Length];
        for (int i = 0; i < sections.Length; i++) {
            // There are three cases here:
            if (highlighted == null) {
                // 1. No panel is highlighted. In this case, pull all other panels to the right.
                endCoord[i] = i == typeSectionDictionary[type] ? 0 : rightBound;
            } else if (highlighted == type) {
                // 2. One panel is highlighted, and it is the panel specified by index.
                // In this case, pull the other panels back.
                endCoord[i] = 0;
            } else {
                // 3. One panel is hightlighted, but it is not the panel specified by index.
                // In this case, pull the currently highlighted panel to the right,
                // and pull the index panel back.
                endCoord[i] = i == typeSectionDictionary[type] ? 0 : rightBound;
            }
        }

        while (timePassed < animationTime) {
            for (int i = 0; i < sectionRT.Length; i++) {
                float anchoredX = Mathf.Lerp(sectionRT[i].anchoredPosition.x, endCoord[i], 0.08f);
                sectionRT[i].anchoredPosition = new Vector2(anchoredX, sectionY[i]);
            }
            timePassed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < sectionRT.Length; i++) {
            sectionRT[i].anchoredPosition = new Vector2(endCoord[i], sectionY[i]);
        }

        highlighted = highlighted == type ? null : (HealthType?)type;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For a detail panel with sections that can be added or subtracted.
/// </summary>
public class ModularPanel : MonoBehaviour {
    public GameObject[] sections;

    public static readonly Dictionary<HealthType, int> typeSectionDictionary = new Dictionary<HealthType, int> {
        {HealthType.overall, 0},
        {HealthType.bodyFatMass, 1},
        {HealthType.bmi, 2},
        {HealthType.aic, 3},
        {HealthType.ldl, 4},
        {HealthType.sbp, 5}
   };

    public void SetBounds() {
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            LinearIndicatorSlideBarManager manager = sections[pair.Value].GetComponent<IndicatorPanelItem>().slideBarManager as LinearIndicatorSlideBarManager;

            Gender gender = HumanManager.Instance.SelectedArchetype.gender;

            manager.min = BiometricContainer.Instance.GetRange(pair.Key, gender).min;
            manager.max = BiometricContainer.Instance.GetRange(pair.Key, gender).max;

            // for body fat, males and females have differnt warning and upper values.
            float warning = BiometricContainer.Instance.GetRange(pair.Key, gender).warning;
            float upper = BiometricContainer.Instance.GetRange(pair.Key, gender).upper;
            manager.warning = warning;
            manager.upper = upper;

            manager.SetBackground();
        }

    }

    /// <summary>
    /// Sets the values.
    /// </summary>
    /// <param name="index">Index, a.k.a. year value.</param>
    public void SetValues(int index) {
        Gender gender = HumanManager.Instance.SelectedArchetype.gender;
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            IndicatorPanelItem item = sections[pair.Value].GetComponent<IndicatorPanelItem>();
            if (pair.Key != HealthType.overall) {
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.None].typeDataDictionary[pair.Key][index], 0);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Minimal].typeDataDictionary[pair.Key][index], 1);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Optimal].typeDataDictionary[pair.Key][index], 2);
            } else {
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.None].CalculateHealth(index, gender), 0);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Minimal].CalculateHealth(index, gender), 1);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Optimal].CalculateHealth(index, gender), 2);
            }
        }
    }

    /// <summary>
    /// Hide/Show the specific section.
    /// </summary>
    /// <returns>true if the section has been shown, else false.</returns>
    /// <param name="index">Index of the section.</param>
    public void Toggle(int index, bool isOn) {
        sections[index].gameObject.SetActive(isOn);
    }

    /// <summary>
    /// Hide/Show the section's background.
    /// </summary>
    /// <param name="on">If set to <c>true</c> enable.</param>
    public void ToggleAllBackground(bool on) {
        foreach (GameObject section in sections) {
            section.GetComponent<Image>().enabled = on;
        }
    }

    /// <summary>
    /// Hide/Show the bar's normal, warning and upper colors.
    /// </summary>
    /// <param name="on">If set to <c>true</c> on.</param>
    public void ToggleAllBars(bool on) {
        foreach (GameObject section in sections) {
            LinearIndicatorSlideBarManager manager = section.GetComponent<IndicatorPanelItem>().slideBarManager as LinearIndicatorSlideBarManager;
            manager.background.ToggleBackground(on);
        }
    }

    /// <summary>
    /// Toggle section color between light/dark.
    /// </summary>
    /// <param name="on">If set to <c>true</c> turn to dark.</param>
    public void ToggleColor(bool on) {
        foreach (GameObject section in sections) {
            LinearIndicatorSlideBarManager manager = section.GetComponent<IndicatorPanelItem>().slideBarManager as LinearIndicatorSlideBarManager;
            manager.background.ToggleBackgroundColor(on);
        }
    }

    public IEnumerator PullSection(int index, bool on) {
        float endCoord = on ? 1100f : 0f;
        float animationTime = 2.0f;

        float timePassed = 0;

        RectTransform rec = sections[index].GetComponent<RectTransform>();
        float top, bottom, left, right;
        // Ribbon charts
        while (timePassed < animationTime) {
            top = rec.offsetMax.y;
            bottom = rec.offsetMin.y;

            left = Mathf.Lerp(rec.offsetMin.x, endCoord, 0.08f);
            right = Mathf.Lerp(rec.offsetMax.x, endCoord, 0.08f);

            // panel themselves
            rec.offsetMin = new Vector2(left, bottom);
            rec.offsetMax = new Vector2(right, top);

            timePassed += Time.deltaTime;
            yield return null;
        }

        top = rec.offsetMax.y;
        bottom = rec.offsetMin.y;

        left = endCoord;
        right = endCoord;

        rec.offsetMin = new Vector2(left, bottom);
        rec.offsetMax = new Vector2(right, top);
        yield return null;
    }
}

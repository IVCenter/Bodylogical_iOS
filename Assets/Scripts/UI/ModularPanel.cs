using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For a detail panel with sections that can be added or subtracted.
/// </summary>
public class ModularPanel : MonoBehaviour {
    public GameObject[] sections;

    public float animationTime = 2.0f;

    public static readonly Dictionary<HealthType, int> typeSectionDictionary = new Dictionary<HealthType, int> {
        { HealthType.overall, 0 },
        { HealthType.bodyFatMass, 1 },
        { HealthType.bmi, 2 },
        { HealthType.aic, 3 },
        { HealthType.ldl, 4 },
        { HealthType.sbp, 5 }
   };

    /// <summary>
    /// Sets the min, max, warning and upper bounds for the sliders.
    /// </summary>
    public void SetBounds() {
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            LinearIndicatorSlideBarManager manager = sections[pair.Value].GetComponent<IndicatorPanelItem>().slideBarManager as LinearIndicatorSlideBarManager;

            Gender gender = HumanManager.Instance.selectedArchetype.gender;

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
    public void SetValues(int index, HealthChoice choice) {
        Gender gender = HumanManager.Instance.selectedArchetype.gender;
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            IndicatorPanelItem item = sections[pair.Value].GetComponent<IndicatorPanelItem>();
            if (pair.Key != HealthType.overall) {
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[choice].typeDataDictionary[pair.Key][index]);
            } else {
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index, gender));
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
            LinearIndicatorSlideBarManager manager = section.GetComponent<IndicatorPanelItem>().slideBarManager as LinearIndicatorSlideBarManager;
            manager.background.ToggleBackground(!on);
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

    /// <summary>
    /// Pulls a section to the right of the panel, or restore it to the original
    /// position.
    /// </summary>
    /// <param name="index">Index of the panel.</param>
    /// <param name="on">If set to <c>true</c> pull to the right.
    /// If false, restore to the left.</param>
    public IEnumerator PullSection(int index, bool on) {
        float endCoord = on ? 1100f : 0f;
        float timePassed = 0;

        RectTransform rec = sections[index].GetComponent<RectTransform>();
        float anchoredY = rec.anchoredPosition.y;

        while (timePassed < animationTime) {
            float anchoredX = Mathf.Lerp(rec.anchoredPosition.x, endCoord, 0.08f);
            // panel themselves
            rec.anchoredPosition = new Vector2(anchoredX, anchoredY);

            timePassed += Time.deltaTime;
            yield return null;
        }

        rec.anchoredPosition = new Vector2(endCoord, anchoredY);
    }

    
}

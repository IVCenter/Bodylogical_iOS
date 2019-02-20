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

            manager.min = BiometricContainer.Instance.StatusRangeDictionary[pair.Key].min;
            manager.max = BiometricContainer.Instance.StatusRangeDictionary[pair.Key].max;

            // for body fat, males and females have differnt warning and upper values.
            bool alt = HumanManager.Instance.SelectedArchetype.sex == "female";
            float calcWarn = alt ?
              BiometricContainer.Instance.StatusRangeDictionary[pair.Key].warningAlt :
              BiometricContainer.Instance.StatusRangeDictionary[pair.Key].warning;
            float calcUpper = alt ?
              BiometricContainer.Instance.StatusRangeDictionary[pair.Key].upperAlt :
              BiometricContainer.Instance.StatusRangeDictionary[pair.Key].upper;
            manager.warning = calcWarn;
            manager.upper = calcUpper;

            manager.SetBackground();
        }

    }

    /// <summary>
    /// Sets the values.
    /// </summary>
    /// <param name="index">Index, a.k.a. year value.</param>
    public void SetValues(int index) {
        foreach (KeyValuePair<HealthType, int> pair in typeSectionDictionary) {
            IndicatorPanelItem item = sections[pair.Value].GetComponent<IndicatorPanelItem>();
            if (pair.Key != HealthType.overall) {
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.None].typeDataDictionary[pair.Key][index], 0);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Minimal].typeDataDictionary[pair.Key][index], 1);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Recommended].typeDataDictionary[pair.Key][index], 2);
            } else {
                bool alt = HumanManager.Instance.SelectedArchetype.sex == "female";
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.None].CalculateHealth(index, alt), 0);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Minimal].CalculateHealth(index, alt), 1);
                item.SetValue(HealthDataContainer.Instance.choiceDataDictionary[HealthChoice.Recommended].CalculateHealth(index, alt), 2);
            }
        }
    }

    /// <summary>
    /// Hide/Show the specific section.
    /// </summary>
    /// <returns>true if the section has been shown, else false.</returns>
    /// <param name="index">Index of the section.</param>
    public bool Toggle(int index) {
        if (sections[index].gameObject.activeSelf) { // hide
            Hide(index);
        } else {
            Show(index);
        }

        return sections[index].gameObject.activeSelf;
    }

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

    public void SeparateSections(bool on) {
        for (int i = 0; i < sections.Length; i++) {
            StartCoroutine(MoveSection(i, on));
        }
    }

    private IEnumerator MoveSection(int index,  bool on) {
        bool isLeft = index % 2 != 0;

        float endLeft, endRight;
        float animationTime = 2.0f;

        if (isLeft) {
            endLeft = -400f;
            endRight = -400f;
        } else {
            endLeft = 400f;
            endRight = 400f;
        }

        if (!on) {
            endLeft = 0f;
            endRight = 0f;
        }

        float timePassed = 0;

        RectTransform rec = sections[index].GetComponent<RectTransform>();
        float top, bottom, left, right;
        while (timePassed < animationTime) {
            // Ribbon charts
            MeshRenderer[] res = rec.gameObject.transform.GetComponentsInChildren<MeshRenderer>();

            top = rec.offsetMax.y;
            bottom = rec.offsetMin.y;

            left = Mathf.Lerp(rec.offsetMin.x, endLeft, 0.08f);
            right = Mathf.Lerp(rec.offsetMax.x, endRight, 0.08f);

            float deltaleft = left - rec.offsetMin.x;

            foreach (MeshRenderer m in res) {
                float newX = m.gameObject.transform.localPosition.x + deltaleft;
                float newY = m.gameObject.transform.localPosition.y;
                float newZ = m.gameObject.transform.localPosition.z;
                m.gameObject.transform.localPosition = new Vector3(newX, newY, newZ);
            }

            // panel themselves
            rec.offsetMin = new Vector2(left, bottom);
            rec.offsetMax = new Vector2(right, top);

            timePassed += Time.deltaTime;
            yield return null;
        }

        top = rec.offsetMax.y;
        bottom = rec.offsetMin.y;

        left = endLeft;
        right = endRight;

        rec.offsetMin = new Vector2(left, bottom);
        rec.offsetMax = new Vector2(right, top);

        yield return null;
    }
}

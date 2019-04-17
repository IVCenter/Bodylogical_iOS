﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The manager to control the Prius visualization, a.k.a. internals.
/// </summary>
public class PriusManager : MonoBehaviour {
    public static PriusManager Instance { get; private set; }

    public GameObject femaleXRay, maleXRay;
    public GameObject priusParent;
    public GameObject priusSelections;
    public GameObject smallHeartGroup, smallLiverGroup;
    public GameObject largeHeartGroup, largeLiverGroup;
    public GameObject smallLeftKidneyGroup, smallRightKidneyGroup;
    public GameObject largeKidneyGroup;
    public GameObject canvas;

    public GameObject LegendPanel { get { return canvas.transform.Search("Legend Panel").gameObject; } }
    public Text ExplanationText { get { return canvas.transform.Search("Explanation Text").GetComponent<Text>(); } }
    public PriusVisualizer Visualizer { get { return priusSelections.GetComponent<PriusVisualizer>(); } }
    [HideInInspector]
    public PriusType currentPart;
    public DropDownInteract showStatusInteract;
    public PriusShowStatus ShowStatus { get { return (PriusShowStatus)showStatusInteract.currIndex; } }
    public Text showStatusText;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public IEnumerator StartPrius() {
        currentPart = PriusType.Human;
        Visualizer.Initialize();
        Visualize(TimeProgressManager.Instance.YearCount / 5, TimeProgressManager.Instance.Path);
        SetExplanationText();
        yield return null;
    }

    /// <summary>
    /// Hide/Show all related buttons and items.
    /// </summary>
    public void TogglePrius(bool on) {
        ButtonSequenceManager.Instance.SetPriusButton(!on);

        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetLineChartButton(on);
        ButtonSequenceManager.Instance.SetActivitiesButton(on);
        ButtonSequenceManager.Instance.SetTimeControls(on);
        ButtonSequenceManager.Instance.SetPriusFunction(on);

        // if toggle off, hide both models; else show the one with the corresponding gender.
        bool isFemale = HumanManager.Instance.SelectedArchetype.sex == Gender.Female;
        maleXRay.SetActive(!isFemale && on);
        femaleXRay.SetActive(isFemale && on);

        HumanManager.Instance.SelectedHuman.SetActive(!on); // x-ray replaces model
        priusParent.SetActive(on);
    }

    /// <summary>
    /// When the heart part is clicked, move the heart to the top, and show the circulation.
    /// </summary>
    public void ToggleHeart() {
        bool isHeart = currentPart == PriusType.Heart;
        currentPart = isHeart ? PriusType.Human : PriusType.Heart;
        smallLiverGroup.SetActive(true);
        largeLiverGroup.SetActive(false);
        smallLeftKidneyGroup.SetActive(true);
        smallRightKidneyGroup.SetActive(true);
        largeKidneyGroup.SetActive(false);
        LegendPanel.SetActive(isHeart);
        Visualizer.MoveOrgan(!isHeart, currentPart, smallHeartGroup, largeHeartGroup);
        SetExplanationText();
    }

    /// <summary>
    /// When the kidney part is clicked, move the kidney to the top, and show the kidney.
    /// </summary>
    public void ToggleKidney(bool left) {
        bool prevLeft = Visualizer.KidneyLeft;
        Visualizer.KidneyLeft = left;
        bool isKidney = currentPart == PriusType.Kidney;
        bool differed = prevLeft != left;

        // if previous is kidney but is a DIFFERENT kidney, still keep current part
        // and play small-to-large animation
        bool stl = !isKidney || (prevLeft != left);

        currentPart = !stl ? PriusType.Human : PriusType.Kidney;
        smallHeartGroup.SetActive(true);
        largeHeartGroup.SetActive(false);
        smallLiverGroup.SetActive(true);
        largeLiverGroup.SetActive(false);
        if (left) {
            smallRightKidneyGroup.SetActive(true);
        } else {
            smallLeftKidneyGroup.SetActive(true);
        }
        if (stl) {
            largeKidneyGroup.SetActive(false);
        }
        LegendPanel.SetActive(isKidney);

        if (left) {
            Visualizer.MoveOrgan(stl, currentPart, smallLeftKidneyGroup, largeKidneyGroup);
        } else {
            Visualizer.MoveOrgan(stl, currentPart, smallRightKidneyGroup, largeKidneyGroup);
        }
        SetExplanationText();
    }

    /// <summary>
    /// WHen the liver part is clicked, move the liver to the top, and show the liver.
    /// </summary>
    public void ToggleLiver() {
        bool isLiver = currentPart == PriusType.Liver;
        currentPart = isLiver ? PriusType.Human : PriusType.Liver;
        smallHeartGroup.SetActive(true);
        largeHeartGroup.SetActive(false);
        smallLeftKidneyGroup.SetActive(true);
        smallRightKidneyGroup.SetActive(true);
        largeKidneyGroup.SetActive(false);
        LegendPanel.SetActive(isLiver);
        Visualizer.MoveOrgan(!isLiver, currentPart, smallLiverGroup, largeLiverGroup);
        SetExplanationText();
    }

    /// <summary>
    /// Play the prius visualization.
    /// </summary>
    /// <returns><c>true</c> if the something so important happens that the time progression needs to be paused for closer inspection.</returns>
    public bool Visualize(int index, HealthChoice choice) {
        return Visualizer.Visualize(index, choice);
    }

    /// <summary>
    /// Sets the explanation text.
    /// </summary>
    public void SetExplanationText() {
        ExplanationText.text = Visualizer.ExplanationText;
    }

    public void SwitchShowStatus(int index) {
        showStatusText.text = "Current: " + ShowStatus.ToString();
        Visualizer.ShowOrgan(currentPart);
    }

    public void Reset() {
        showStatusInteract.OnOptionClicked(0);
    }
}
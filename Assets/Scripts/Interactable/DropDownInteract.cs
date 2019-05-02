using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropDownInteract : FoldablePanelInteract {
    public Color selectedColor, unselectedColor;

    public int currIndex;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    [Header("Value changed. Indicate what to happen.")]
    public IntEvent onChange;

    private List<DropDownOptionInteract> interacts;

    void Awake() {
        interacts = new List<DropDownOptionInteract>();
        foreach (Transform optionTransform in canvas.transform.GetChild(0)) {
            interacts.Add(optionTransform.GetComponent<DropDownOptionInteract>());
        }

        for (int i = 0; i < interacts.Count; i++) {
            interacts[i].index = i;
            interacts[i].panel.color = unselectedColor;
            interacts[i].clicked.AddListener(OnOptionClicked);
        }

        interacts[currIndex].panel.color = selectedColor;
    }

    public void OnOptionClicked(int index) {
        interacts[currIndex].panel.color = unselectedColor;
        interacts[index].panel.color = selectedColor;
        interacts[index].originalColor = selectedColor;
        currIndex = index;
        onChange.Invoke(index);
    }
}


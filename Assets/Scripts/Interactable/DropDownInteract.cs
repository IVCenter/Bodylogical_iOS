using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class DropDownInteract : FoldablePanelInteract {
    public Color selectedColor, unselectedColor;
    public GameObject[] options;
    public int currIndex;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    public IntEvent onChange;

    private DropDownOptionInteract[] interacts;


    void Awake() {
        interacts = options.Select(option => option.GetComponent<DropDownOptionInteract>()).ToArray();
        for (int i = 0; i < interacts.Length; i++) {
            interacts[i].index = i;
            interacts[i].panel.color = unselectedColor;
            interacts[i].clicked.AddListener(OnOptionClicked);
        }

        interacts[currIndex].panel.color = selectedColor;
    }

    public void OnOptionClicked(int index) {
        print(index);
        interacts[currIndex].panel.color = unselectedColor;
        interacts[index].panel.color = selectedColor;
        currIndex = index;
        onChange.Invoke(index);
    }
}


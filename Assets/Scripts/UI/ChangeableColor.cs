using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeableColor : MonoBehaviour {
    public Color original, alternative;

    private Image Background { get { return GetComponent<Image>(); } }

    public void ToggleColor(bool on) {
        Background.color = on ? alternative : original;
    }
}

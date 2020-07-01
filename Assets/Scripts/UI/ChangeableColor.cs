using UnityEngine;
using UnityEngine.UI;

public class ChangeableColor : MonoBehaviour {
    [SerializeField] private Color original, alternative;
    private Image Background => GetComponent<Image>();

    public void ToggleColor(bool on) {
        Background.color = on ? alternative : original;
    }
}

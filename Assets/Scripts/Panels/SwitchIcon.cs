using UnityEngine;
using UnityEngine.UI;

public class SwitchIcon : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activity, prius, stats;

    private void Start() {
        icon.gameObject.SetActive(false);
    }

    public void Switch() {
        
    }
}

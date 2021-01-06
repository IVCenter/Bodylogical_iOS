using UnityEngine;
using UnityEngine.UI;

public class SwitchIcon : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activity, prius, stats;

    private void Start() {
        icon.gameObject.SetActive(false);
    }

    /// <summary>
    /// This will be called in Details mode; show the Activity icon.
    /// </summary>
    public void Initialize() {
        icon.gameObject.SetActive(true);
        icon.sprite = activity;
    }

    public void Switch() {
        if (AppStateManager.Instance.CurrState == AppState.Idle) {
            //DetailPanel.Instance.ToggleDetailPanel(false);
            StageManager.Instance.SwitchActivity();
            // Switched to Activity, next is Prius.
            icon.sprite = prius;
        } else if (AppStateManager.Instance.CurrState == AppState.VisLineChart) {
            StageManager.Instance.SwitchActivity();
            // Switched to Activity, next is Prius.
            icon.sprite = prius;
        } else if (AppStateManager.Instance.CurrState == AppState.VisActivity) {
            StageManager.Instance.SwitchPrius();
            // Switched to Prius, next is Stats.
            icon.sprite = stats;
        } else if (AppStateManager.Instance.CurrState == AppState.VisPrius) {
            StageManager.Instance.SwitchLineChart();
            // Switched to Stats, next is Activity.
            icon.sprite = activity;
        }
    }
}

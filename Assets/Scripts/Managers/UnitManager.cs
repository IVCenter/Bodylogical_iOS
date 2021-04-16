using UnityEngine;

/// <summary>
/// Handles unit input and conversion in the front end. The back end uses SI internally.
/// Places where we need to take care of:
/// - Basic info panel/Data panel, where the users enters the height and weight;
/// - The four detail panels, which shows the user's future weight.
/// </summary>
public class UnitManager : MonoBehaviour {
    public static UnitManager Instance { get; private set; }

    [SerializeField] private DataPanel dataPanel;

    [System.Serializable]
    private class SliderConfig {
        public DetailPanel detailPanel;
        public CircularSlideBarManager slideBarManager;
    }

    // We assume that the first value in these slide bars is the weight value.
    [SerializeField] private SliderConfig[] sliders;

    private Unit currentUnit = Unit.Imperial;
    private HealthRange weightSIRange;
    private HealthRange weightImperialRange;
    private HealthRange CurrRange => currentUnit == Unit.SI ? weightSIRange : weightImperialRange;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        weightSIRange = HealthUtil.GetRange(HealthType.weight);
        weightImperialRange = new HealthRange {
            min = GetWeight(weightSIRange.min),
            warning = GetWeight(weightSIRange.warning),
            danger = GetWeight(weightSIRange.danger),
            max = GetWeight(weightSIRange.max)
        };
        
        HealthRange range = CurrRange;
        foreach (SliderConfig slider in sliders) {
            slider.slideBarManager.SetRange(0, range.min, range.warning, range.danger, range.max);
        }
    }

    public int GetWeight(float kg) =>
        currentUnit == Unit.SI ? Mathf.RoundToInt(kg) : Conversion.KgToLb(Mathf.RoundToInt(kg));

    public void ChangeUnit(Unit unit) {
        if (unit == currentUnit) {
            return;
        }

        currentUnit = unit;

        HealthRange range = CurrRange;
        foreach (SliderConfig slider in sliders) {
            slider.slideBarManager.SetRange(0, range.min, range.warning, range.danger, range.max);
            slider.detailPanel.UpdateStats();
        }

        // TODO: data panel
    }
}
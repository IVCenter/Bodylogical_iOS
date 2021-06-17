using UnityEngine;

/// <summary>
/// Handles unit input and conversion in the front end. The back end uses SI internally.
/// Places where we need to take care of:
/// - Basic info panel/Data panel, where the users enters the height and weight;
/// - The four detail panels, which shows the user's future weight.
/// </summary>
public class UnitManager : MonoBehaviour {
    public static UnitManager Instance { get; private set; }
    public Unit CurrentUnit { get; private set; } = Unit.Imperial;

    [SerializeField] private BasicInfoPanel basicInfoPanel;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public int GetWeight(float kg) =>
        CurrentUnit == Unit.SI ? Mathf.RoundToInt(kg) : Conversion.KgToLb(Mathf.RoundToInt(kg));

    public void ChangeUnit(Unit unit) {
        if (unit == CurrentUnit) {
            return;
        }

        CurrentUnit = unit;

        basicInfoPanel.SwitchUnit();
    }
}
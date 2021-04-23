using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataPanel : MonoBehaviour {
    [SerializeField] private InputField userName, age, heightFt, heightInch, heightCm, weightLb, weightKg;
    [SerializeField] private GameObject heightImperial, heightSI, weightImperial, weightSI;
    [SerializeField] private Toggle maleToggle;
    [SerializeField] private Button reset, confirm;

    private string Name => userName.text;
    private Gender Sex => maleToggle.isOn ? Gender.Male : Gender.Female;
    private int Age => int.Parse(age.text);

    // inch can be empty, but ft must be filled.
    private int Height => UnitManager.Instance.CurrentUnit == Unit.SI
        ? int.Parse(heightCm.text)
        : Conversion.FtInchToCm(int.Parse(heightFt.text), TryParse(heightInch.text));

    private int Weight => UnitManager.Instance.CurrentUnit == Unit.SI
        ? int.Parse(weightKg.text)
        : Conversion.LbToKg(int.Parse(weightLb.text));

    /// <summary>
    /// When the user changes the weight, update the avatar's blend shape to reflect the change.
    /// </summary>
    public void OnWeightChanged(string value) {
        if (value == "") { // Clear changes
            ArchetypeManager.Instance.displayer.SetWeight(0);
        }

        ArchetypeManager.Instance.displayer.SetWeight(Weight);
    }

    public void LockButtons(bool on) {
        reset.interactable = !on;
        confirm.interactable = !on;
    }

    public void ResetFields() {
        userName.text = "";
        age.text = "";
        heightFt.text = "";
        heightInch.text = "";
        heightCm.text = "";
        weightLb.text = "";
        weightKg.text = "";

        ArchetypeManager.Instance.displayer.SetWeight(0);
    }

    public void Confirm() {
        if (CheckError()) {
            return;
        }

        Archetype archetype = new Archetype {
            name = Name,
            gender = Sex,
            age = Age,
            height = Height,
            weight = Weight
        };

        ArchetypeManager.Instance.displayer.ArchetypeData = archetype;
        AppStateManager.Instance.CurrState = AppState.ShowDetails; // Trigger change
    }

    /// <summary>
    /// Checks the input for errors. All fields need to be nonempty (temporary).
    /// </summary>
    /// <returns>false if there are no errors, true otherwise.</returns>
    private bool CheckError() {
        // TODO: inches should not exceed 12?
        return !(userName.text != "" && age.text != "" &&
                 (UnitManager.Instance.CurrentUnit == Unit.Imperial && heightFt.text != "" && weightLb.text != "" ||
                  UnitManager.Instance.CurrentUnit == Unit.SI && heightCm.text != "" &&
                  weightKg.text != ""));
    }

    public void ChangeUnit() {
        UnitManager.Instance.ChangeUnit(UnitManager.Instance.CurrentUnit == Unit.Imperial ? Unit.SI : Unit.Imperial);
    }

    public void SwitchUnit() {
        if (UnitManager.Instance.CurrentUnit == Unit.SI) {
            heightSI.SetActive(true);
            heightImperial.SetActive(false);
            weightSI.SetActive(true);
            weightImperial.SetActive(false);

            if (int.TryParse(heightFt.text, out int ft)) {
                heightCm.text = Conversion.FtInchToCm(ft, TryParse(heightInch.text)).ToString();
            }

            if (int.TryParse(weightLb.text, out int lb)) {
                weightKg.text = Conversion.LbToKg(lb).ToString();
            }
        } else {
            heightSI.SetActive(false);
            heightImperial.SetActive(true);
            weightSI.SetActive(false);
            weightImperial.SetActive(true);

            if (int.TryParse(heightCm.text, out int cm)) {
                KeyValuePair<int, int> ftin = Conversion.CmtoFtInch(cm);
                heightFt.text = ftin.Key.ToString();
                heightInch.text = ftin.Value.ToString();
            }

            if (int.TryParse(weightKg.text, out int kg)) {
                weightLb.text = Conversion.KgToLb(kg).ToString();
            }
        }
    }

    private int TryParse(string text) => text == "" ? 0 : int.Parse(text);
}
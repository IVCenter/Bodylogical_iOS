using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MonoBehaviour {
    [SerializeField] private InputField userName, age, heightFt, heightInch, weight;
    [SerializeField] private Toggle maleToggle;
    [SerializeField] private Button reset, confirm; // TODO: 
    
    public string Name => userName.text;
    public Gender Sex => maleToggle.isOn ? Gender.Male : Gender.Female;
    public int Age => int.Parse(age.text);

    public int Height => Mathf.RoundToInt((int.Parse(heightFt.text) * 12 + int.Parse(heightInch.text)) * 2.54f);

    public int Weight => Mathf.RoundToInt((int.Parse(weight.text)) * 0.45f);

    public void LockButtons(bool on) {
        
    }
    
    public void ResetFields() {
        userName.text = "";
        age.text = "";
        heightFt.text = "";
        heightInch.text = "";
        weight.text = "";
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
        // TODO: make it more flexible
        // TODO: lock submit/reset
        // TODO: inches should not exceed 12?
    }

    /// <summary>
    /// Checks the input for errors. All fields need to be nonempty (temporary).
    /// </summary>
    /// <returns>false if there are no errors, true otherwise.</returns>
    private bool CheckError() {
        return !(userName.text != "" && age.text != "" && heightFt.text != "" && heightInch.text != "" &&
                 weight.text != "");
    }
}
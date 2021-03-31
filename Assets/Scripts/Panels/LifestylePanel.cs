using UnityEngine;
using UnityEngine.UI;

public class LifestylePanel : MonoBehaviour {
    [SerializeField] private InputField exercise, sleep, carb, fat, protein;
    [SerializeField] private Button reset, confirm;

    // TODO: replace with slide bars/other UI element
    private float Exercise => float.Parse(exercise.text);
    private float Sleep => float.Parse(sleep.text);
    private int Carb => int.Parse(carb.text);
    private int Fat => int.Parse(fat.text);
    private int Protein => int.Parse(protein.text);

    public void LockButtons(bool on) {
        reset.interactable = on;
        confirm.interactable = on;
    }

    public void ResetLifestyle() {
        exercise.text = "";
        sleep.text = "";
        carb.text = "";
        fat.text = "";
        protein.text = "";
    }

    public void Confirm() {
        if (CheckError()) {
            return;
        }

        Lifestyle lifestyle = new Lifestyle {
            sleepHours = Sleep,
            exercise = Exercise,
            carbIntake = Carb,
            fatIntake = Fat,
            proteinIntake = Protein
        };
        
        // TODO: update lifestyle and make web request
    }

    /// <summary>
    /// Checks the input for errors. All fields need to be nonempty (temporary).
    /// </summary>
    /// <returns>false if there are no errors, true otherwise.</returns>
    private bool CheckError() {
        return !(sleep.text != "" && exercise.text != "" && carb.text != "" && fat.text != "" && protein.text != "");
    }
}
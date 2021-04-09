using System.Collections;
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
        reset.interactable = !on;
        confirm.interactable = !on;
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

        StartCoroutine(SubmitLifestyle(new Lifestyle {
            sleepHours = Sleep,
            exercise = Exercise,
            carbIntake = Carb,
            fatIntake = Fat,
            proteinIntake = Protein
        }));
    }

    private IEnumerator SubmitLifestyle(Lifestyle lifestyle) {
        // Lock the buttons and show a loading text
        ControlPanelManager.Instance.LPanel.LockButtons(true);
        TutorialManager.Instance.ShowInstruction("Instructions.CalculateData");

        NetworkError error = new NetworkError();
        yield return ArchetypeManager.Instance.Performer.QueryHealth(error, lifestyle);
        
        // Unlock the buttons and hide loading text
        ControlPanelManager.Instance.LPanel.LockButtons(false);
        TutorialManager.Instance.ClearInstruction();
        
        // Unlock the buttons and hide loading text
        if (error.status == NetworkStatus.Success) {
            // Show the data on the panel
            ArchetypeManager.Instance.Performer.UpdateStats();
        } else {
            // Request error
            StartCoroutine(ShowErrorInstruction(error));
        }
    }
    
    private IEnumerator ShowErrorInstruction(NetworkError error) {
        Debug.Log(error.message);
        TutorialManager.Instance.ShowInstruction(error.MsgKey);
        yield return new WaitForSeconds(5);
        TutorialManager.Instance.ClearInstruction();
    }

    /// <summary>
    /// Checks the input for errors. All fields need to be nonempty (temporary).
    /// </summary>
    /// <returns>false if there are no errors, true otherwise.</returns>
    private bool CheckError() {
        return !(sleep.text != "" && exercise.text != "" && carb.text != "" && fat.text != "" && protein.text != "");
    }
}
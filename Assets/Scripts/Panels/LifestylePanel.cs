using System.Collections;
using UnityEngine;

public class LifestylePanel : MonoBehaviour {
    [SerializeField] private SliderValueAdapter exercise, sleep, carb, fat, protein;
    [SerializeField] private ButtonInteract reset, confirm;

    public void LockButtons(bool on) {
        reset.Enabled = !on;
        confirm.Enabled = !on;
    }

    public void ResetFields() {
        exercise.Slider.SetSlider(0.5f);
        sleep.Slider.SetSlider(0.5f);
        carb.Slider.SetSlider(0.5f);
        fat.Slider.SetSlider(0.5f);
        protein.Slider.SetSlider(0.5f);
    }

    public void Confirm() {
        StartCoroutine(SubmitLifestyle(new Lifestyle {
            sleepHours = sleep.Value,
            exercise = exercise.Value,
            carbIntake = Mathf.RoundToInt(carb.Value),
            fatIntake = Mathf.RoundToInt(fat.Value),
            proteinIntake = Mathf.RoundToInt(protein.Value)
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
}
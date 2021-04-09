using UnityEngine;

/// <summary>
/// A base wrapper class for the display models.
/// </summary>
public class ArchetypeDisplayer : ArchetypeModel {
    public VisualizeIcon icon;
    private static readonly int Greetings = Animator.StringToHash("Greetings");

    public void SetGreetingPose(bool on) {
        Anim.SetBool(Greetings, on);
    }

    public void Reset() {
        SetGreetingPose(false);
        panel.Reset();
        icon.ResetIcon();
    }
}
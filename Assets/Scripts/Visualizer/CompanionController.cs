using UnityEngine;

public class CompanionController : MonoBehaviour {
    public Gender gender;
    public Renderer companionRenderer;
    public Material CompanionMaterial => companionRenderer.material;
    public Animator companionAnimator;
    public Texture normalTexture, agedTexture;
    public GameObject legend;

    private static readonly int oldYear = 15;

    public void SetTexture(float year) {
        CompanionMaterial.mainTexture = (year >= oldYear) ? agedTexture : normalTexture;
    }

    public void ToggleLegend(bool on) {
        legend.SetActive(on);
    }
}

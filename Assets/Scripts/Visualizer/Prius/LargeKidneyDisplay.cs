using UnityEngine;

public class LargeKidneyDisplay : OrganDisplay {
    public GameObject kidney;
    public Material urineMaterial;
    public Color normalColor, badColor;


    public override void DisplayOrgan(int score) {
        kidney.SetActive(true);
        urineMaterial.color = Color.Lerp(badColor, normalColor, score / 100.0f);
    }
}

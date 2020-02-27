using UnityEngine;

public class LargeLiverDisplay : OrganDisplay {
    public GameObject liver;

    public SkinnedMeshRenderer LiverRenderer => liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

    public override void DisplayOrgan(int score) {
        LiverRenderer.SetBlendShapeWeight(0, 100 - score);
    }
}

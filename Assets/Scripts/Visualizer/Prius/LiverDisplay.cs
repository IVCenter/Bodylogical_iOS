using UnityEngine;

public class LiverDisplay : OrganDisplay {
    public GameObject liver;

    public SkinnedMeshRenderer LiverRenderer => liver.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

    public override void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            LiverRenderer.SetBlendShapeWeight(0, 100 - score);
        }
    }
}

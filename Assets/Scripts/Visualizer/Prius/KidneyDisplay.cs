using UnityEngine;

public class KidneyDisplay : OrganDisplay {
    public GameObject leftKidney, rightKidney;

    private SkinnedMeshRenderer LeftRenderer => leftKidney.GetComponent<SkinnedMeshRenderer>();
    private SkinnedMeshRenderer RightRenderer => rightKidney.GetComponent<SkinnedMeshRenderer>();

    public override void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            // Both kidneys will be shown.
            leftKidney.SetActive(true);
            rightKidney.SetActive(true);
            LeftRenderer.SetBlendShapeWeight(0, 100 - score);
            RightRenderer.SetBlendShapeWeight(0, 100 - score);
        }
    }
}

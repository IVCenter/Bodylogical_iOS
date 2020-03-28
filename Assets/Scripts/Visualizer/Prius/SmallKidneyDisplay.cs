using UnityEngine;

public class SmallKidneyDisplay : OrganDisplay {
    public GameObject leftKidney, rightKidney;

    private SkinnedMeshRenderer LeftRenderer => leftKidney.GetComponent<SkinnedMeshRenderer>();
    private SkinnedMeshRenderer RightRenderer => rightKidney.GetComponent<SkinnedMeshRenderer>();

    public override void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            // Only OtherKidney is shown; CurrKidney is enlarged and will
            // be managed by LargeKidneyDisplay, so it will be hidden.
            if (PriusManager.Instance.currentPart == PriusType.Kidney) {
                leftKidney.SetActive(false);
                rightKidney.SetActive(true);
                // Blend shape for the kidney is from "good" to "bad".
                // 0 means good, and 100 means bad.
                // So we need to reverse the score.
                RightRenderer.SetBlendShapeWeight(0, 100 - score);
            } else {
                // Both kidneys will be shown.
                leftKidney.SetActive(true);
                rightKidney.SetActive(true);
                LeftRenderer.SetBlendShapeWeight(0, 100 - score);
                RightRenderer.SetBlendShapeWeight(0, 100 - score);
            }

        }
    }
}

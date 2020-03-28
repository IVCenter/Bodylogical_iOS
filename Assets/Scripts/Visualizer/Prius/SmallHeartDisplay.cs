using UnityEngine;

public class SmallHeartDisplay : OrganDisplay {
    public GameObject heart;
    private Animator HeartAnimator => heart.transform.GetChild(0).GetComponent<Animator>();

    public override void DisplayOrgan(int score) {
        if (gameObject.activeInHierarchy) {
            heart.SetActive(true);
            HeartAnimator.speed = 1.0f - score / 100.0f;
        }
    }
}

using UnityEngine;

public class WheelchairController : MonoBehaviour {
    [SerializeField] private Transform pusherTransform;

    private CompanionController companion;
    private static readonly int pushWheelchair = Animator.StringToHash("PushWheelchair");

    public void Initialize() {
        companion = Instantiate(
            ArchetypeManager.Instance.Selected.ArchetypeData.gender == Gender.Female
            ? ActivityManager.Instance.femaleCompanionPrefab
            : ActivityManager.Instance.maleCompanionPrefab, pusherTransform, false)
            .GetComponent<CompanionController>();
        companion.companionAnimator.SetBool(pushWheelchair, true);
    }
}

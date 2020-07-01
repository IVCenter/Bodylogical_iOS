using UnityEngine;

public class WheelchairController : MonoBehaviour {
    [SerializeField] private Transform pusherTransform;

    private CompanionController companion;

    public void Initialize() {
        companion = Instantiate(
            ArchetypeManager.Instance.Selected.ArchetypeData.gender == Gender.Female
            ? ActivityManager.Instance.femaleCompanionPrefab
            : ActivityManager.Instance.maleCompanionPrefab)
            .GetComponent<CompanionController>();
        companion.transform.SetParent(pusherTransform, false);
        companion.companionAnimator.SetTrigger("PushWheelchair");
    }

    public void Dispose() {
        Destroy(companion.gameObject);
    }
}

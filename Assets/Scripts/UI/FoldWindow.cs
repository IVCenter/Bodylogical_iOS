using System.Collections;
using UnityEngine;

public class FoldWindow : MonoBehaviour {
    [SerializeField] private float animationTime = 2f;
    private bool isAnimating;
    private Vector3 initialLocalPos;

    private void Awake() {
        initialLocalPos = transform.localPosition;
    }

    private void OnEnable() {
        if (!isAnimating) {
            StartCoroutine(AxisDriftingZ());
        }
    }

    private IEnumerator AxisDriftingZ() {
        isAnimating = true;

        Vector3 start = new Vector3(initialLocalPos.x, initialLocalPos.y, -0.5f);

        transform.localPosition = start;
        float timePassed = 0;

        while (timePassed < animationTime) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, 0.08f);
            timePassed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialLocalPos;
        isAnimating = false;
        yield return null;
    }
}

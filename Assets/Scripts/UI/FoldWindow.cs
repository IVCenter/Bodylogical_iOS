using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FoldWindow : MonoBehaviour {
    public float animationTime = 2f;
    private bool isAnimating;
    private Vector3 initialLocalPos;

    void Awake() {
        isAnimating = false;

        initialLocalPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        //initial_localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void OnEnable() {
        //DebugText.Instance.Log("Onenabled");
        if (!isAnimating) {
            //DebugText.Instance.Log("started bloom");
            StartCoroutine(AxisDriftingZ());
        }
    }

    IEnumerator AxisDriftingZ() {
        isAnimating = true;

        Vector3 start = new Vector3(initialLocalPos.x, initialLocalPos.y, -0.5f);

        transform.localPosition = start;
        //transform.localScale = Vector3.zero;

        float timePassed = 0;

        while (timePassed < animationTime) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, 0.08f);
            //transform.localScale = Vector3.Lerp(transform.localScale, initial_localScale, 0.08f);

            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initialLocalPos;
        //transform.localScale = initial_localScale;

        //DebugText.Instance.Log("inital local scale: " + initial_localScale);

        isAnimating = false;

        yield return null;
    }
}

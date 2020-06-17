using System.Collections;
using UnityEngine;

public class FoldWindow : MonoBehaviour {
    public float animation_time = 2f;
    private bool isAnimating;
    private Vector3 initial_localPos;
    //private Vector3 initial_localScale;

    void Awake() {
        isAnimating = false;

        initial_localPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
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

        Vector3 start = new Vector3(initial_localPos.x, initial_localPos.y, -0.5f);

        transform.localPosition = start;
        //transform.localScale = Vector3.zero;

        float time_passed = 0;

        while (time_passed < animation_time) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initial_localPos, 0.08f);
            //transform.localScale = Vector3.Lerp(transform.localScale, initial_localScale, 0.08f);

            time_passed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initial_localPos;
        //transform.localScale = initial_localScale;

        //DebugText.Instance.Log("inital local scale: " + initial_localScale);

        isAnimating = false;

        yield return null;
    }
}

using System.Collections;
using UnityEngine;
public class PulseIcon : MonoBehaviour {
    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] private float duration = 2f;

    private IEnumerator pulse;
    private Vector3 original;
    
    private void Awake() {
        original = transform.localScale;
    }

    public void StartPulse() {
        pulse = Pulse();
        StartCoroutine(pulse);
    }

    public void StopPulse() {
        if (pulse != null) {
            StopCoroutine(pulse);
            pulse = null;
            transform.localScale = original;
        }
    }

    private IEnumerator Pulse() {
        Vector3 target = original * scaleFactor;
        Vector3 velocity = Vector3.zero;
        while (true) {
            // Original to target
            float time = duration;
            transform.localScale = original;
            while (time > 0) {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, target, ref velocity, time);
                time -= Time.deltaTime;
                yield return null;
            }

            // Target to original
            time = duration;
            transform.localScale = target;
            while (time > 0) {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, original, ref velocity, time);
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
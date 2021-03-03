using System.Collections;
using UnityEngine;

public class LockRotation : MonoBehaviour {
    private Quaternion rotation;
    private IEnumerator lockCoroutine;
    
    private void Start() {
        rotation = transform.rotation;
    }

    public void StartLock() {
        if (gameObject.activeSelf) {
            lockCoroutine = Lock();
            StartCoroutine(lockCoroutine);
        }
    }

    public void EndLock() {
        if (lockCoroutine != null) {
            StopCoroutine(lockCoroutine);
            lockCoroutine = null;
        }
    }
    
    private IEnumerator Lock() {
        while (true) {
            transform.rotation = rotation;
            yield return null;
        }
    }
}
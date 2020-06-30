using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }
    public float maxScale;
    public bool PlaneFound { get; private set; }
    private List<GameObject> planes;
    private bool isConfirming;
    private PlaneFinder finder;
    private IEnumerator scan;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        finder = GetComponent<PlaneFinder>();

    }

    private IEnumerator Scan() {
        while (true) {
            if (!PlaneFound) {
                if (finder.planes.Count > 0) {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneGood");
                    isConfirming = true;
                }
            }

            yield return null; // Defer to next frame

            if (isConfirming) {
                if (InputManager.Instance.TouchCount > 0 &&
                    InputManager.Instance.TapCount > 1) { // Double tap
                    TutorialManager.Instance.ClearInstruction();
                    planes = GetComponent<PlaneFinder>().Finish();

                    PlaneFound = true;
                    isConfirming = false;
                }
            }

            yield return null;
        }
    }

    public void BeginScan() {
        GetComponent<PlaneFinder>().Begin();
        //finding = true;
        scan = Scan();
        StartCoroutine(scan);
    }

    public void EndScan() {
        if (scan != null) {
            StopCoroutine(scan);
            scan = null;
        }
    }

    public void RestartScan() {
        gameObject.GetComponent<PlaneFinder>().Reset();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        isConfirming = false;
        PlaneFound = false;
        scan = Scan();
        StartCoroutine(scan);
    }

    public void HidePlanes() {
        if (planes != null) { // If in debug/editor, MainPlane will be null.
            foreach (GameObject p in planes) {
                p.SetActive(false);
            }
        }
    }
}

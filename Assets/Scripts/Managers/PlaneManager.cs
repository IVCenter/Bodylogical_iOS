using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }

    public float maxScale;

    [HideInInspector] public bool finding;

    public bool PlaneFound { get; private set; }

    private List<GameObject> planes;

    private bool isConfirming;

    private PlaneFinder finder;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        finder = GetComponent<PlaneFinder>();

    }

    // Update is called once per frame
    private void Update() {
        if (finding) {
            if (!PlaneFound) {
                if (finder.planes.Count > 0) {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneGood");
                    isConfirming = true;
                } else {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneCont");
                }
            }

            if (isConfirming) {
                if (InputManager.Instance.TouchCount > 0) {
                    TutorialManager.Instance.ClearInstruction();
                    planes = GetComponent<PlaneFinder>().Finish();
                    
                    PlaneFound = true;
                    isConfirming = false;
                }
            }
        }
    }

    public void BeginScan() {
        GetComponent<PlaneFinder>().Begin();
        finding = true;
    }

    public void RestartScan() {
        gameObject.GetComponent<PlaneFinder>().Reset();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        isConfirming = false;
        PlaneFound = false;
    }

    public void HidePlanes() {
        if (planes != null) { // If in debug/editor, MainPlane will be null.
            foreach (GameObject p in planes) {
                p.SetActive(false);
            }
        }
    }
}

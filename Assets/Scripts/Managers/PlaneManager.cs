using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }

    [HideInInspector]
    public bool finding;

    public bool PlaneFound { get; private set; }

    public GameObject MainPlane { get; private set; }

    private bool isConfirming;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    /// <summary>
    /// Sets up the instruction.
    /// </summary>
    void Start() {
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
    }

    // Update is called once per frame
    void Update() {
        if (finding) {
            if (!PlaneFound) {
                float scale = gameObject.GetComponent<FindLargestPlane>().GetCurrentLargestPlaneScale();
                // DebugText.Instance.Log("plane scale is: " + scale);
                if (scale > 0.007) {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneGood");
                    isConfirming = true;
                } else if (scale > 0.001) {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneCont");
                }
            }

            if (isConfirming) {
                if ((InputManager.Instance.TouchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown("space")) {
                    TutorialManager.Instance.ClearInstruction();
                    MainPlane = gameObject.GetComponent<FindLargestPlane>().FinishProcess();
                    PlaneFound = true;
                    isConfirming = false;
                }
            }
        }
    }

    public void RestartScan() {
        gameObject.GetComponent<FindLargestPlane>().RestartProcess();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        isConfirming = false;
        PlaneFound = false;
    }

    public void HideMainPlane() {
        MainPlane.GetComponentInChildren<MeshRenderer>().enabled = false;
        MainPlane.GetComponentInChildren<BoxCollider>().enabled = false;
    }
}

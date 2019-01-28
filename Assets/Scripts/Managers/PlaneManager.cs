﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }

    public Text userNotification;

    public bool PlaneFound {
        get; private set;
    }

    public GameObject MainPlane {
        get; private set;
    }

    private bool isConfirming = false;

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
        userNotification.text = "Please find a flat surface";
    }

    // Update is called once per frame
    void Update() {
        if (!PlaneFound) {
            float scale = gameObject.GetComponent<FindLargestPlane>().GetCurrentLargestPlaneScale();
            // DebugText.Instance.Log("plane scale is: " + scale);
            if (scale > 0.007) {
                userNotification.text = "This Plane looks good. Confirm?";
                isConfirming = true;
            } else if (scale > 0.001) {
                userNotification.text = "Continue Scanning...";
            }
        }

        if (isConfirming) {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.N)) {
                userNotification.text = "";
                MainPlane = gameObject.GetComponent<FindLargestPlane>().FinishProcess();
                PlaneFound = true;
                isConfirming = false;
            }
        }
    }

    public void RestartScan() {
        gameObject.GetComponent<FindLargestPlane>().RestartProcess();
        userNotification.text = "Please find a flat surface";
        isConfirming = false;
        PlaneFound = false;
    }

    public void HideMainPlane() {
        MainPlane.GetComponentInChildren<MeshRenderer>().enabled = false;
        MainPlane.GetComponentInChildren<BoxCollider>().enabled = false;
    }
}
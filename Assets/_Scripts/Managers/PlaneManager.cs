using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneManager : MonoBehaviour {

    public Text userNotification;

    public static PlaneManager Instance;
    private bool foundPlane = false;
    private GameObject mainPlane;
    private bool isConfirming = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        userNotification.text = "Please find a flat surface";
    }
	
	// Update is called once per frame
	void Update () {

        if(isConfirming){
            if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.N)){
                userNotification.text = "";
                mainPlane = gameObject.GetComponent<FindLargestPlane>().FinishProcess();
                foundPlane = true;
                isConfirming = false;
            }

        }

        if (!foundPlane)
        {
            float scale = gameObject.GetComponent<FindLargestPlane>().GetCurrentLargestPlaneScale();
            // DebugText.Instance.Log("plane scale is: " + scale);
            if (scale > 0.007){
                userNotification.text = "This Plane looks good. Confirm?";
                isConfirming = true;
            }
            else if (scale > 0.001){
                userNotification.text = "Continue Scanning...";
            }
        }
	}

    public void RestartScan(){
        gameObject.GetComponent<FindLargestPlane>().RestartProcess();
        userNotification.text = "Please find a flat surface";
        isConfirming = false;
        foundPlane = false;
    }

    public bool IsPlaneFound(){
        return foundPlane;
    }

    public GameObject GetMainPlane(){
        return mainPlane;
    }

}

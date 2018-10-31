using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    private GameObject focusedObj;
    private Material myMat;
    private Vector3 original_scale;
    private Vector3 idle_scale;
    private Color original_color;
    private Color highlight_color;

    private bool isIdle;
    private bool isTouching;

    // Use this for initialization
    void Start () {

        if (gameObject.GetComponent<MeshRenderer>() != null)
        {
            myMat = gameObject.GetComponent<MeshRenderer>().material;
        }
        else{
            myMat = gameObject.AddComponent<MeshRenderer>().material;
        }

        original_color = myMat.color;
        original_scale = transform.localScale;
        idle_scale = new Vector3(0.02f, 0.02f, 0.02f);
        highlight_color = new Color(255 / 255.0f, 255 / 255.0f, 51 / 255.0f);
        focusedObj = null;
        isIdle = true;
        isTouching = false;
    }


    // Update is called once per frame
    void Update () {
        
        RaycastTest();

        TouchTest();

    }

    void RaycastTest(){

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            gameObject.transform.position = hit.point;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (isIdle)
            { 
                BecomeBusy(); 
            }

            if (focusedObj == null || focusedObj != hit.transform.gameObject)
            {
                RegisterFocusedObj(hit);
            }
        }
        else
        {
            gameObject.transform.position = Camera.main.transform.TransformDirection(Vector3.forward) * 1.5f + Camera.main.transform.position;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward.normalized * 1.5f, Color.white);

            if(!isIdle)
            {
                BecomeIdle();
            }

            if (focusedObj != null)
            {
                FreeFocusedObj();
            }
        }
    }

    void BecomeBusy(){
        myMat.color = highlight_color;
        gameObject.transform.localScale = original_scale;
        isIdle = false;
    }

    void BecomeIdle(){
        myMat.color = original_color;
        gameObject.transform.localScale = idle_scale;
        isIdle = true;
    }

    void RegisterFocusedObj(RaycastHit hit){

        if (focusedObj != null)
        {
            // quit old object
            if (focusedObj.GetComponent(typeof(IInteractible)) != null)
            {
                IInteractible interactive = (IInteractible)focusedObj.GetComponent(typeof(IInteractible));
                interactive.OnCursorExited();
            }
        }

        focusedObj = hit.transform.gameObject;

        // inform new object
        if (focusedObj.GetComponent(typeof(IInteractible)) != null)
        {
            IInteractible interactive = (IInteractible)focusedObj.GetComponent(typeof(IInteractible));
            interactive.OnCursorEnter();
        }

        DebugText.Instance.Log("CurrentObj: " + focusedObj.name);
    }

    void FreeFocusedObj(){

        if (focusedObj != null){
            // quit old object
            if (focusedObj.GetComponent(typeof(IInteractible)) != null)
            {
                IInteractible interactive = (IInteractible)focusedObj.GetComponent(typeof(IInteractible));
                interactive.OnCursorExited();
            }

            focusedObj = null;
        }
    }

    void TouchTest(){

        isTouching = Input.touchCount > 0;

        if (isTouching && Input.GetTouch(0).phase == TouchPhase.Began){

            DebugText.Instance.Log("Touched");

            if (focusedObj.GetComponent(typeof(IInteractible)) != null)
            {
                IInteractible interactive = (IInteractible)focusedObj.GetComponent(typeof(IInteractible));
                interactive.OnScreenTouch(Input.GetTouch(0).position);
            }

        }

        if (isTouching && Input.GetTouch(0).phase == TouchPhase.Moved){

            if (focusedObj.GetComponent(typeof(IInteractible)) != null)
            {
                IInteractible interactive = (IInteractible)focusedObj.GetComponent(typeof(IInteractible));

                // pass the status of finger touch
                interactive.OnScreenTouchMoved(Input.GetTouch(0).position, Input.GetTouch(0).deltaPosition);
            }
        }

        if (isTouching && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {

            if (focusedObj.GetComponent(typeof(IInteractible)) != null)
            {
                IInteractible interactive = (IInteractible)focusedObj.GetComponent(typeof(IInteractible));

                // pass the status of finger touch
                interactive.OnScreenPress(Input.GetTouch(0).position, Input.GetTouch(0).deltaTime, 
                                          Input.GetTouch(0).pressure);
            }
        }


    }


    public GameObject GetCurrentFocusedObj(){
        return focusedObj;
    }

    public bool CursorIdle(){
        return isIdle;
    }

    public Vector3 GetCursorPosition(){
        return transform.position;
    }
}

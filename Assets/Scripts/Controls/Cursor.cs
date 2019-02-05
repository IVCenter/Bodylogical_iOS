using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public GameObject FocusedObj { get; private set; }
    public Vector3 CursorPosition {
        get { return transform.position; }
    }

    private Material myMat;
    private Vector3 original_scale;
    private Vector3 idle_scale;
    private Color original_color;
    private Color highlight_color;

    private bool isIdle;

    /// <summary>
    /// If the user is grabbing an object, like a slider's knob.
    /// </summary>
    private bool isHolding;

    // Use this for initialization
    void Start() {
        if (gameObject.GetComponent<MeshRenderer>() != null) {
            myMat = gameObject.GetComponent<MeshRenderer>().material;
        } else {
            myMat = gameObject.AddComponent<MeshRenderer>().material;
        }

        original_color = myMat.color;
        original_scale = transform.localScale;
        idle_scale = new Vector3(0.02f, 0.02f, 0.02f);
        highlight_color = new Color(255 / 255.0f, 255 / 255.0f, 51 / 255.0f);
        FocusedObj = null;
        isIdle = true;
    }


    // Update is called once per frame
    void Update() {
        RaycastTest();
        TouchTest();
    }

    void RaycastTest() {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
            gameObject.transform.position = hit.point;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (isIdle) {
                BecomeBusy();
            }

            if (FocusedObj == null || FocusedObj != hit.transform.gameObject) {
                RegisterFocusedObj(hit);
            }
        } else {
            gameObject.transform.position = Camera.main.transform.TransformDirection(Vector3.forward) * 1.5f + Camera.main.transform.position;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward.normalized * 1.5f, Color.white);

            if (!isIdle) {
                BecomeIdle();
            }

            if (FocusedObj != null && !isHolding) {
                FreeFocusedObj();
            }
        }
    }

    void BecomeBusy() {
        myMat.color = highlight_color;
        gameObject.transform.localScale = original_scale;
        isIdle = false;
    }

    void BecomeIdle() {
        myMat.color = original_color;
        gameObject.transform.localScale = idle_scale;
        isIdle = true;
    }

    void RegisterFocusedObj(RaycastHit hit) {
        if (FocusedObj != null) {
            // quit old object
            if (FocusedObj.GetComponent(typeof(IInteractable)) != null) {
                IInteractable interactive = (IInteractable)FocusedObj.GetComponent(typeof(IInteractable));
                interactive.OnCursorExited();
            }
        }

        FocusedObj = hit.transform.gameObject;

        // inform new object
        if (FocusedObj.GetComponent(typeof(IInteractable)) != null) {
            IInteractable interactive = (IInteractable)FocusedObj.GetComponent(typeof(IInteractable));
            interactive.OnCursorEnter();
        }

        DebugText.Instance.Log("CurrentObj: " + FocusedObj.name);
    }

    void FreeFocusedObj() {
        if (FocusedObj != null) {
            // quit old object
            if (FocusedObj.GetComponent(typeof(IInteractable)) != null) {
                IInteractable interactive = (IInteractable)FocusedObj.GetComponent(typeof(IInteractable));
                interactive.OnCursorExited();
            }

            FocusedObj = null;
        }
    }

    void TouchTest() {
        if (FocusedObj == null) {
            return;
        }

        if (FocusedObj.GetComponent(typeof(IInteractable)) != null) {
            IInteractable interactive = (IInteractable)FocusedObj.GetComponent(typeof(IInteractable));
            if (Input.touchCount > 0) { // Formal use: screen press
                switch (Input.GetTouch(0).phase) {
                    case TouchPhase.Began:
                        interactive.OnScreenTouch(Input.GetTouch(0).position);
                        break;

                    case TouchPhase.Moved:
                        interactive.OnScreenTouchMoved(Input.GetTouch(0).position, Input.GetTouch(0).deltaPosition);
                        break;

                    case TouchPhase.Stationary:
                        isHolding = true;
                        interactive.OnScreenPress(Input.GetTouch(0).position, Input.GetTouch(0).deltaTime,
                                                  Input.GetTouch(0).pressure);
                        break;

                    case TouchPhase.Ended:
                        isHolding = false;
                        interactive.OnScreenLeave(Input.GetTouch(0).position);
                        break;
                }
            } else { // debug use: keyboard press. Does NOT support OnScreenTouchMoved.
                if (Input.GetKeyDown("space")) {
                    interactive.OnScreenTouch(new Vector2());
                }

                if (Input.GetKey("space")) {
                    interactive.OnScreenPress(new Vector2(), 0, 0);
                }

                if (Input.GetKeyUp("space")) {
                    interactive.OnScreenLeave(new Vector2());
                }
            }
        }
    }
}

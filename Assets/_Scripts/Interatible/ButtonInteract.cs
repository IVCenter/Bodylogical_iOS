using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteract : MonoBehaviour, IInteractible {

    [Header("This button is clicked. Indicate what to happen.")]
    public UnityEvent clicked;

    private Color origin_color;

    private void Awake()
    {
        origin_color = GetComponent<MeshRenderer>().material.color;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCursorEnter(){
        //Debug.Log("Cursor Entered");
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void OnCursorExited(){
        //Debug.Log("Cursor Exited");
        gameObject.GetComponent<MeshRenderer>().material.color = origin_color;
    }

    public void OnScreenTouch(Vector2 coord)
    {
        clicked.Invoke();
        //DebugText.Instance.Log("Touched on object: " + gameObject.name);
        //DebugText.Instance.Log("Touched coord is : " + coord);
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure)
    {
        //DebugText.Instance.Log("Pressed on object: " + gameObject.name);
        //DebugText.Instance.Log("Pressed pressure: " + pressure);
    }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition)
    {
        //DebugText.Instance.Log("Touch moved delta " + deltaPosition);
    }


}

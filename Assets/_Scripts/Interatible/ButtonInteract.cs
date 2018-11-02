using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteract : MonoBehaviour, IInteractible {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCursorEnter(){
        //Debug.Log("Cursor Entered");
    }

    public void OnCursorExited(){
        //Debug.Log("Cursor Exited");
    }

    public void OnScreenTouch(Vector2 coord)
    {
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

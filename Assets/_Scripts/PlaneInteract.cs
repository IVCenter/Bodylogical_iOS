using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneInteract : MonoBehaviour, IInteractible {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCursorEnter()
    {
        //Debug.Log("Cursor Entered");
    }

    public void OnCursorExited()
    {
        //Debug.Log("Cursor Exited");
    }

    public void OnScreenTouch(Vector2 coord)
    {
        //DebugText.Instance.Log("Touched on object: " + gameObject.name);
        //DebugText.Instance.Log("Touched coord is : " + coord);

        Vector3 cursorPos = CursorManager.Instance.cursor.GetCursorPosition();
        GameObject humanModel = HumanManager.Instance.humanModel;
        Transform footPoint = humanModel.transform.GetChild(0);
        Vector3 diff = humanModel.transform.position - footPoint.position;

        humanModel.transform.position = cursorPos + diff;

        DebugText.Instance.Log("Touched PlaneInteract Get Called");
        DebugText.Instance.Log("The HumanModel is: " + humanModel);
        DebugText.Instance.Log("The Cursor Pos is: " + CursorManager.Instance.cursor.GetCursorPosition());
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

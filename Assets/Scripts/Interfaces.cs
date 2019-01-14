using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object must be interactible with cursor
public interface IInteractible {

    void OnCursorEnter();

    void OnCursorExited();

    void OnScreenTouch(Vector2 coord);

    void OnScreenPress(Vector2 coord, float deltaTime, float pressure);

    void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition);

}


// Quick Template:

//public void OnCursorEnter()
//{
//    //Debug.Log("Cursor Entered");
//}

//public void OnCursorExited()
//{
//    //Debug.Log("Cursor Exited");
//}

//public void OnScreenTouch(Vector2 coord)
//{
//    //DebugText.Instance.Log("Touched on object: " + gameObject.name);
//    //DebugText.Instance.Log("Touched coord is : " + coord);
//}

//public void OnScreenPress(Vector2 coord, float deltaTime, float pressure)
//{
//    //DebugText.Instance.Log("Pressed on object: " + gameObject.name);
//    //DebugText.Instance.Log("Pressed pressure: " + pressure);
//}

//public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition)
//{
//    //DebugText.Instance.Log("Touch moved delta " + deltaPosition);
//}
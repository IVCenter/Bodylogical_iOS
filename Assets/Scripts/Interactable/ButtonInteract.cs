using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class ButtonInteract : MonoBehaviour, IInteractable {
    [Header("This button is clicked. Indicate what to happen.")]
    public UnityEvent clicked;

    private Color? originalColor;

    public void OnCursorEnter() {
        //Debug.Log("Cursor Entered");
        if (gameObject.GetComponent<MeshRenderer>()) {
            if (originalColor == null) {
                originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
            }

            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public void OnCursorExited() {
        //Debug.Log("Cursor Exited");
        if (GetComponent<MeshRenderer>() && originalColor != null) {
            GetComponent<MeshRenderer>().material.color = (Color)originalColor;
            originalColor = null;
        }
    }

    public virtual void OnScreenTouch(Vector2 coord) {
        clicked.Invoke();
        //DebugText.Instance.Log("Touched on object: " + gameObject.name);
        //DebugText.Instance.Log("Touched coord is : " + coord);
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) {
        //DebugText.Instance.Log("Pressed on object: " + gameObject.name);
        //DebugText.Instance.Log("Pressed pressure: " + pressure);
    }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) {
        //DebugText.Instance.Log("Touch moved delta " + deltaPosition);
    }

    public void OnScreenLeave(Vector2 coord) { }
}

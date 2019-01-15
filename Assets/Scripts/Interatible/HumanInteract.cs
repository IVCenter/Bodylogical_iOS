using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInteract : MonoBehaviour, IInteractible {

    public Material highlight;

    [HideInInspector]
    public bool isSelected;

    // TODO: Original_mats won't work on objects composed of multiple parts
    //private Material[] original_mats;

    // Use this for initialization
    void Start() {
        //original_mats = gameObject.GetComponent<MeshRenderer>().materials;
        isSelected = false;
    }

    public void OnCursorEnter() {
        Debug.Log("Cursor Entered");
        //Material[] array = gameObject.GetComponent<MeshRenderer>().materials;
        //array[2] = highlight;
        //gameObject.GetComponent<MeshRenderer>().materials = array;
    }

    public void OnCursorExited() {
        Debug.Log("Cursor Exited");
        //gameObject.GetComponent<MeshRenderer>().materials = original_mats;
    }

    public void OnScreenTouch(Vector2 coord) {
        //DebugText.Instance.Log("Touched on object: " + gameObject.name);
        //DebugText.Instance.Log("Touched coord is : " + coord);

        if (MasterManager.Instance.CurrGamePhase == MasterManager.GamePhase.Phase3) {
            DebugText.Instance.Log("A human is selected");
            isSelected = true;
        }
    }

    public void OnScreenPress(Vector2 coord, float deltaTime, float pressure) {
        //DebugText.Instance.Log("Pressed on object: " + gameObject.name);
        //DebugText.Instance.Log("Pressed pressure: " + pressure);
    }

    public void OnScreenTouchMoved(Vector2 coord, Vector2 deltaPosition) {
        //DebugText.Instance.Log("Touch moved delta " + deltaPosition);
    }
}

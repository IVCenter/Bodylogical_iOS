using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class ButtonInteract : MonoBehaviour, IInteractible {


    public enum ButtonType { Regular, DynamicSpawn }

    public ButtonType type = ButtonType.Regular;

    public int choiceType = 0;

    [Header("This button is clicked. Indicate what to happen.")]
    public UnityEvent clicked;



    private Color origin_color;

    private void Awake() {
        if (gameObject.GetComponent<MeshRenderer>()) {
            origin_color = GetComponent<MeshRenderer>().material.color;
        }
    }

    // Use this for initialization
    void Start() {
        if (type == ButtonType.DynamicSpawn) {
            clicked.AddListener(delegate { HumanManager.Instance.FireNextPeriod(choiceType); });
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnCursorEnter() {
        //Debug.Log("Cursor Entered");
        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public void OnCursorExited() {
        //Debug.Log("Cursor Exited");
        if (gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().material.color = origin_color;
        }
    }

    public void OnScreenTouch(Vector2 coord) {
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


}

//[CustomEditor(typeof(ButtonInteract))]
//public class MyScriptEditor : Editor
//{
//    override public void OnInspectorGUI()
//    {
//        var myScript = target as ButtonInteract;

//        myScript.flag = GUILayout.Toggle(myScript.flag, "Flag");

//        if (myScript.flag)
//            myScript.i = EditorGUILayout.IntSlider("I field:", myScript.i, 1, 100);

//    }
//}

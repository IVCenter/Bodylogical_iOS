using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragRotate : MonoBehaviour
{
    public GameObject subject;
    public string currentPoint = "Default";
    /*public Vector3 initCoord = new Vector3(0, 0, 0);
    public bool reset = true;

    void Start()
    {
        initCoord = subject.transform.position;
    }*/

    // Update is called once per frame
    void Update()
    {
        /*if (subject.activeInHierarchy && reset)
        {
            reset = false;
        }

        else if (!subject.activeInHierarchy && !reset)
        {
            subject.transform.position = initCoord;
            reset = true;
        }*/

        if (Input.touchCount == 1)
        {
            Touch touchZero = Input.GetTouch(0);
            currentPoint = "Current touch point: " + touchZero.position;
            Debug.Log(currentPoint);

            Vector3 dirVector = new Vector3(touchZero.position.x - touchZero.deltaPosition.x, touchZero.position.y - touchZero.deltaPosition.y, 0);
            float touchZeroMag = (touchZero.position - touchZero.deltaPosition).magnitude;

            if (touchZeroMag > 25.0f)
                subject.GetComponent<Rigidbody>().AddTorque((dirVector * touchZeroMag * Time.deltaTime)/2.5f, ForceMode.VelocityChange);

            else if (touchZero.deltaPosition.x < touchZero.position.x || touchZero.deltaPosition.y < touchZero.position.y)
                subject.GetComponent<Rigidbody>().AddTorque((Vector3.Reflect(dirVector, subject.GetComponent<Vector3>()) * touchZeroMag * Time.deltaTime / 2.5f), ForceMode.VelocityChange);
        }
    }
}

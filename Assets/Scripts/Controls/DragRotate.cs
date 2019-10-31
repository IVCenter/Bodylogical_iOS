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

            Vector3 dirVector = new Vector3(touchZero.deltaPosition.y, -touchZero.deltaPosition.x, 0);
            //Vector3 antiDirVector = new Vector3(touchZero.deltaPosition.y, touchZero.deltaPosition.x, 0);
            float touchZeroMag = (touchZero.deltaPosition).magnitude;

            if (touchZeroMag > 4.0f)
                subject.GetComponent<Rigidbody>().AddTorque(dirVector * touchZeroMag * Time.deltaTime * 0.7f, ForceMode.VelocityChange);

            //else if (touchZeroMag > 7.0f && touchZero.deltaPosition.x < 0 || touchZero.deltaPosition.y < 0)
            //    subject.GetComponent<Rigidbody>().AddTorque(dirVector * touchZeroMag * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}

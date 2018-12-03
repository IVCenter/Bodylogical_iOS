using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineControl : MonoBehaviour {

    private GameObject line;
	// Use this for initialization
	void Start () {
        line = DrawLine(Vector3.zero, Vector3.up, Color.green, gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        // Keep rendered line in line with actual line.
        Vector3 gazeLoc;
        gazeLoc = GameObject.Find("DefaultCursor").transform.position;
        line.GetComponent<LineRenderer>().SetPosition(1, gazeLoc);
	}

    GameObject DrawLine(Vector3 start, Vector3 end, Color color, GameObject parent)
    {
        GameObject LineObject = new GameObject("LineObject");
        GameObject line = Instantiate(LineObject, parent.transform);
        Destroy(LineObject);

        line.transform.position = start;
        LineRenderer lineRend = line.AddComponent<LineRenderer>();
        lineRend.startColor = color;
        lineRend.endColor = color;
        lineRend.startWidth = 0.01f;
        lineRend.endWidth = 0.01f;
        lineRend.SetPosition(0, start);
        lineRend.SetPosition(1, end);

        return line;
    }
}

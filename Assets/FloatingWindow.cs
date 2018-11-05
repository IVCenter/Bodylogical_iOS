using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingWindow : MonoBehaviour {

    public float X_range = 0.2f, Y_range = 0.3f;

    private float X_left, X_right, Y_up, Y_down;

    private float z;
    private float frame_counter;
    private Vector3 down_des, up_des;
    private bool isUp;
    private float threshold_time; 

	// Use this for initialization
	void Start () {
        X_left = transform.localPosition.x - X_range;
        X_right = transform.localPosition.x + X_range;
        Y_up = transform.localPosition.y + Y_range;
        Y_down = transform.localPosition.y - Y_range;

        z = transform.localPosition.z;
        frame_counter = 0;
        up_des = new Vector3(Random.Range(X_left, X_right), Y_up, z);
        down_des = new Vector3(Random.Range(X_left, X_right), Y_down, z);
        isUp = true;
        threshold_time = Random.Range(2, 4);
    }
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf){
            frame_counter += Time.deltaTime;

            if (frame_counter > threshold_time)
            {
                up_des.x = Random.Range(X_left, X_right);
                down_des.x = Random.Range(X_left, X_right);
                up_des.y = Y_up + Random.Range(-0.05f, 0.05f);
                down_des.y = Y_down + Random.Range(-0.05f, 0.05f);
                frame_counter = 0;
                isUp = !isUp;
                threshold_time = Random.Range(2, 4);
            }

            if (isUp)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, up_des, Time.deltaTime/5);
            }else{
                transform.localPosition = Vector3.Lerp(transform.localPosition, down_des, Time.deltaTime/5);
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingWindow : MonoBehaviour {

    public float X_range = 0.2f, Y_range = 0.3f;
    public float animation_time = 2f;

    private float X_left, X_right, Y_up, Y_down;

    private float z;
    private float frame_counter;
    private Vector3 down_des, up_des;
    private bool isUp;
    private float threshold_time;
    private Vector3 initial_localPos;
    private Vector3 initial_localScale;
    private bool isAnimating;

    private void Awake()
    {
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

        isAnimating = false;

        initial_localPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        initial_localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }

    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf && !isAnimating){
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

    private void OnEnable()
    {
        DebugText.Instance.Log("initial pos: " + initial_localPos);

        if(!isAnimating){
            StartCoroutine(Bloom());
        }
        
    }

    IEnumerator Bloom(){

        isAnimating = true;

        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.zero;

        float time_passed = 0;

        while (time_passed < animation_time){

            transform.localScale = Vector3.Lerp(transform.localScale, initial_localScale, 0.08f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initial_localPos, 0.08f);

            time_passed += Time.deltaTime;

            yield return null;
        }

        transform.localScale = initial_localScale;
        transform.localPosition = initial_localPos;

        isAnimating = false;

        yield return null;
    }
}

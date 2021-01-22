using System.Collections;
using UnityEngine;

public class FloatingWindow : MonoBehaviour {
    [SerializeField] private float xRange = 0.2f;
    [SerializeField] private float yRange = 0.3f;

    private float xLeft, xRight, yUp, yDown;
    private float z;
    private float frameCounter;
    private Vector3 downDes, upDes;
    private bool isUp = true;
    private float thresholdTime;
    private bool isAnimating;

    private void Awake() {
        xLeft = transform.localPosition.x - xRange;
        xRight = transform.localPosition.x + xRange;
        yUp = transform.localPosition.y + yRange;
        yDown = transform.localPosition.y - yRange;

        z = transform.localPosition.z;
        frameCounter = 0;
        upDes = new Vector3(Random.Range(xLeft, xRight), yUp, z);
        downDes = new Vector3(Random.Range(xLeft, xRight), yDown, z);
        thresholdTime = Random.Range(2, 4);
    }

    private void Update() {
        if (gameObject.activeSelf && !isAnimating) {
            frameCounter += Time.deltaTime;

            if (frameCounter > thresholdTime) {
                upDes.x = Random.Range(xLeft, xRight);
                downDes.x = Random.Range(xLeft, xRight);
                upDes.y = yUp + Random.Range(-0.05f, 0.05f);
                downDes.y = yDown + Random.Range(-0.05f, 0.05f);
                frameCounter = 0;
                isUp = !isUp;
                thresholdTime = Random.Range(2, 4);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, isUp ? upDes : downDes, Time.deltaTime / 5);
        }
    }
}
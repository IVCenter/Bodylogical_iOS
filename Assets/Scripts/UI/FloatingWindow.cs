using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FloatingWindow : MonoBehaviour {
    public float xRange = 0.2f;
    public float yRange = 0.3f;
    public float animationTime = 2f;

    private float xLeft, xRight, yUp, yDown;
    private float z;
    private float frameCounter;
    private Vector3 downDes, upDes;
    private bool isUp;
    private float thresholdTime;
    private Vector3 initialLocalPos;
    private Vector3 initialLocalScale;
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
        isUp = true;
        thresholdTime = Random.Range(2, 4);

        isAnimating = false;

        initialLocalPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        initialLocalScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

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

            if (isUp) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, upDes, Time.deltaTime / 5);
            } else {
                transform.localPosition = Vector3.Lerp(transform.localPosition, downDes, Time.deltaTime / 5);
            }
        }
    }

    private void OnEnable() {
        if (!isAnimating) {
            StartCoroutine(Bloom());
        }
    }

    private IEnumerator Bloom() {
        isAnimating = true;

        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.zero;

        float timePassed = 0;

        while (timePassed < animationTime) {
            transform.localScale = Vector3.Lerp(transform.localScale, initialLocalScale, 0.08f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, 0.08f);

            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.localScale = initialLocalScale;
        transform.localPosition = initialLocalPos;

        //DebugText.Instance.Log("inital local scale: " + initial_localScale);

        isAnimating = false;

        yield return null;
    }
}

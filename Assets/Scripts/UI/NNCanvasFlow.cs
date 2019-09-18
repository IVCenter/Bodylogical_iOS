using System.Collections;
using UnityEngine;

public class NNCanvasFlow : Interactable
{
    public RectTransform panel;
    public bool isHidden;
    public bool isAnimating;
    private float canvasScaler;

    public float animateTime;

    public int touches;
    public Vector2[] p = new Vector2[5];
    public float dist1;
    public float dist2;

    void Start()
    {
        panel = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            RunTouch(0);
            RunTouch(1);
        }
    }

    public IEnumerator HideShow()
    {
        isAnimating = true;
        float scaler = isHidden ? panel.transform.localScale.y : 0;
        float timePassed = 0;

        float scaleY = panel.localScale.y;
        while (timePassed < animateTime)
        {
            float scaleX = Mathf.Lerp(panel.localScale.x, scaler, 0.08f);
            panel.localScale = new Vector2(scaleX, scaleY);

            timePassed += Time.deltaTime;
            yield return null;
        }

        panel.localScale = new Vector2(panel.localScale.x, scaler);
        isHidden = !isHidden;
        isAnimating = false;
    }

    public IEnumerator RunTouch(int touchNum)
    {
        Touch touch = Input.GetTouch(touchNum);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                p[touchNum] = touch.position;
                Debug.Log("Begun ");
                break;

            case TouchPhase.Moved:
                p[touchNum] = touch.position;
                Debug.Log("Moving ");
                break;

            case TouchPhase.Ended:
                break;
        }

        yield return null;
    }

    public override void OnTouchDown()
    {
        if (Input.touchCount == 2)
        {
            p[0] = GetTouchXY(0);
            p[1] = GetTouchXY(1);
            dist1 = Mathf.Abs((p[0].x - p[1].x)+(p[0].y - p[1].y));
            Debug.Log("Touches: " + touches);
        }
    }

    public override void OnTouchUp()
    {
        if (Input.touchCount == 0)
        {
            dist2 = Mathf.Abs((p[0].x - p[1].x) + (p[0].y - p[1].y));
            if(dist1.CompareTo(dist2) == 1 && !isAnimating)
            {
                HideShow();
            }
            Debug.Log("Touches: " + touches);
        }
    }

    public Vector2 GetTouchXY(int touch)
    {
        return new Vector2(Input.GetTouch(touch).position.x, Input.GetTouch(touch).position.y);
    }

    /*public override float GetDistance(Vector2 p1, Vector2 p2)
    {
        return Mathf.Abs((p1.x - p2.x) + (p1.y - p2.y));
    }

    public override float GetDistance(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Abs((p1.x - p2.x - p3.x) + (p1.y - p2.y - p3.y) + (p1.z - p2.z - p3.z));
    }*/
}

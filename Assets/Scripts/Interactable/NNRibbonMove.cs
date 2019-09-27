using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NNRibbonMove : MonoBehaviour
{
    //[SerializeField]
    public GameObject[] ribbonGroup = new GameObject[5];
    public GameObject ribbonCenter;
    public GameObject ribbonNext;
    public bool moving = false;
    public float centerCoord;
    public float initialCoord;
    public float nextCoord;
    public static float spacing;
    public static bool on;
    public float moveUpDown;

    public bool run = true;

    // Start is called before the first frame update
    void Start()
    {
        centerCoord = ribbonCenter.transform.position.y;
        initialCoord = ribbonGroup[1].transform.position.y;
        nextCoord = ribbonNext.transform.position.y;
        spacing = initialCoord - nextCoord;
        moveUpDown = on ? 0f : spacing;
    }

    // Update is called once per frame
    void Update()
    {
        moveUpDown = on ? spacing : -spacing;

        //if (run) {
        //    moveTime();
        //    run = false;
        //}

        //if(moving)
        //{
        //    RibbonYShift();
        //}
    }

    public void RibbonYShift()
    {
        on = !on;

        if (!moving)
        {
            moving = true;
            for (int i = ribbonGroup.Length; i >= 0; i--)
            {
                StartCoroutine(moveTime(ribbonGroup[i], moveUpDown * i));
            }
            moving = false;
        }
    }

    IEnumerator moveTime(GameObject panel, float dist)
    {
        moving = true;

        float thisTime = 0f;

        //It does the translation and the RibbonYShift runs through the array and calls this coroutine.
        while (thisTime < 1f) {
            panel.transform.Translate(0, dist * (Time.deltaTime), 0);
            yield return new WaitForEndOfFrame();
            thisTime += Time.deltaTime;
        }
        //yield return new WaitForSeconds(2.0f);

        yield return null;
    }
}

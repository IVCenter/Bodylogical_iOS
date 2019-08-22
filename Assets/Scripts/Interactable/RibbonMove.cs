using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RibbonMove : MonoBehaviour
{
    //[SerializeField]
    public GameObject[] ribbonGroup = new GameObject[5];
    public GameObject ribbonCenter;
    public GameObject ribbonNext;
    public bool moving;
    public float centerCoord;
    public float initialCoord;
    public float nextCoord;
    public float spacing;

    public bool run = true;

    // Start is called before the first frame update
    void Start()
    {
        centerCoord = ribbonCenter.transform.position.y;
        initialCoord = ribbonGroup[1].transform.position.y;
        nextCoord = ribbonNext.transform.position.y;
        spacing = initialCoord - nextCoord;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (!moving)
        {
            for (int i = ribbonGroup.Length; i >= 0; i--)
            {
                moveTime();
                ribbonGroup[i].transform.Translate(0, spacing * i * (Time.deltaTime), 0);
            }
        }
    }

    IEnumerator moveTime()
    {
        moving = true;

        yield return new WaitForSeconds(2.0f);

        moving = false;
    }
}

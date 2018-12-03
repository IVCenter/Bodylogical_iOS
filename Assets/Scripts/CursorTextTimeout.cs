using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorTextTimeout : MonoBehaviour {

    public float timeout = 4.5f;
    private bool counting;
    private Time start;

	void Update () {
        if (!GetComponent<Text>().text.Equals("") && !counting)
        {
            counting = true;
            StartCoroutine(resetText());
        }
	}

    private IEnumerator resetText()
    {
        yield return new WaitForSeconds(timeout);
        GetComponent<Text>().text = "";
        counting = false;
    }
}

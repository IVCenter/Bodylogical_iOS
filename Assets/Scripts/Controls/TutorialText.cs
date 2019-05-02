using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour {
    private Text tutorialText;

    public static TutorialText Instance;

    private IEnumerator coroutine = null;

    private void Awake() {
        tutorialText = GetComponent<Text>();

        if (Instance == null) {
            Instance = this;
        }
    }


    public void Show(string content, float time_to_live) {

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = ShowHelper(content, time_to_live);
        StartCoroutine(coroutine);

    }

    IEnumerator ShowHelper(string content, float time_to_live) {

        tutorialText.text = content;

        yield return new WaitForSeconds(time_to_live);

        tutorialText.text = "";

        coroutine = null;
        yield return null;
    }

    public void ShowDouble(string content, string content2, float time_to_live) {

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = ShowDoubleHelper(content, content2, time_to_live);
        StartCoroutine(coroutine);

    }

    IEnumerator ShowDoubleHelper(string content, string content2, float time_to_live) {

        tutorialText.text = content;

        yield return new WaitForSeconds(time_to_live);

        tutorialText.text = content2;

        yield return new WaitForSeconds(time_to_live);

        tutorialText.text = "";

        coroutine = null;
        yield return null;
    }
}

using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    public LocalizedText instructionText;
    public LocalizedText tutorialText;

    public static TutorialManager Instance;

    private IEnumerator coroutine = null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void ShowInstruction(string content, params LocalizedParam[] param) {
        instructionText.SetText(content, param);
    }

    public void ClearInstruction() {
        instructionText.Clear();
    }

    /// <summary>
    /// Shows a tutorial text for a certain time.
    /// </summary>
    /// <param name="content">Content. Must be already localized (keys are not formatted!)</param>
    /// <param name="timeToLive">Time to live.</param>
    public void Show(string content, float timeToLive) {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = ShowHelper(content, timeToLive);
        StartCoroutine(coroutine);
    }

    IEnumerator ShowHelper(string content, float timeToLive) {
        tutorialText.SetDirectText(content);
        yield return new WaitForSeconds(timeToLive);
        tutorialText.Clear();

        coroutine = null;
        yield return null;
    }

    /// <summary>
    /// Shows two tutorial texts at a given interval.
    /// </summary>
    /// <param name="content">Content. Must be already localized (keys are not formatted!)</param>
    /// <param name="content2">Content2. Must be already localized (keys are not formatted!)</param>
    /// <param name="timeToLive">Time to live.</param>
    public void ShowDouble(string content, string content2, float timeToLive) {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = ShowDoubleHelper(content, content2, timeToLive);
        StartCoroutine(coroutine);

    }

    IEnumerator ShowDoubleHelper(string content, string content2, float timeToLive) {
        tutorialText.SetDirectText(content);
        yield return new WaitForSeconds(timeToLive);
        tutorialText.SetDirectText(content2);
        yield return new WaitForSeconds(timeToLive);
        tutorialText.Clear();

        coroutine = null;
        yield return null;
    }
}

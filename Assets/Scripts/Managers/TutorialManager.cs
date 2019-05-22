using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    public static TutorialManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        tutorialQueue = new Queue<LocalizedGroup[]>();
    }

    #region NEW
    public GameObject tutorialCanvas;
    public LocalizedText canvasText;

    private bool skipAll;
    private bool skipCurrent;
    private bool confirmed;

    private Queue<LocalizedGroup[]> tutorialQueue;
    private IEnumerator currentTutorial;

    /// <summary>
    /// Shows the tutorial of one page.
    /// Notice that this does NOT block.
    /// For blocking, refer to
    /// http://www.feelouttheform.net/unity3d-coroutine-synchronous/
    /// But notice that this won't work with ShowUntil as this equals a while(true)
    /// loop that will never end.
    /// TODO: we might wish to also pass in a System.Func[] as callbacks to each
    /// LocalizedGroup so that something can happen after each user click.
    /// </summary>
    /// <param name="id">id of localized string.</param>
    /// <param name="args">args of id.</param>
    public void ShowTutorial(string id, params LocalizedParam[] args) {
        LocalizedGroup[] groups = { new LocalizedGroup(id, args) };
        ShowTutorial(groups);
    }

    /// <summary>
    /// Shows the tutorial of several pages.
    /// Because the current implementation is asynchronous, if multiple calls to
    /// ShowTutorial() come in in short intervals, they might interfere with each
    /// other. To ensure tutorials goes one-by-one, use queue to cache future tutorials.
    /// </summary>
    /// <param name="groups">Groups.</param>
    public void ShowTutorial(LocalizedGroup[] groups) {
        if (!skipAll) {
            if (currentTutorial == null) {
                currentTutorial = ShowTutorialHelper(groups);
                StartCoroutine(currentTutorial);
                //WaitCoroutine(ShowTutorialHelper(groups));
            } else {
                tutorialQueue.Enqueue(groups);
            }
        } else {
            tutorialQueue.Clear();
        }
    }

    /// <summary>
    /// Controls the actual tutorial.
    /// After the tutorial is completed, invoke the next tutorial, if any exists.
    /// </summary>
    /// <returns>The tutorial helper.</returns>
    /// <param name="groups">Groups.</param>
    private IEnumerator ShowTutorialHelper(LocalizedGroup[] groups) {
        tutorialCanvas.SetActive(true);
        InputManager.Instance.menuOpened = true;
        foreach (LocalizedGroup group in groups) {
            canvasText.SetText(group.id, group.args);
            // The reason we don't use () => confirmed || skipCurrent || skipAll
            // is that *if* we do this we would see one frame of each remaining text
            // whichi is not desired. By setting confirmed to true in button callbacks
            // we can hide all remaining messages completely when the user chooses
            // to skip.
            yield return new WaitUntil(() => confirmed);
            confirmed = false;
            if (skipCurrent || skipAll) {
                break;
            }
        }
        InputManager.Instance.menuOpened = false;
        tutorialCanvas.SetActive(false);
        skipCurrent = false;

        currentTutorial = null;
        // if there exists more tutorials, invoke one.
        if (tutorialQueue.Count != 0) {
            ShowTutorial(tutorialQueue.Dequeue());
        }
    }

    public void SkipAll() {
        skipAll = true;
        confirmed = true;
    }

    public void Next() {
        confirmed = true;
    }

    public void Skip() {
        skipCurrent = true;
        confirmed = true;
    }
    #endregion

    #region OLD
    public LocalizedText instructionText;
    public LocalizedText tutorialText;
    private IEnumerator coroutine = null;

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
    #endregion
}

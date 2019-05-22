using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    public static TutorialManager Instance;

    public LocalizedText instructionText;
    public LocalizedText statusText;
    public GameObject tutorialCanvas;
    public LocalizedText canvasText;
    public GameObject controlButtons;
    public GameObject confirmationPanel;

    private IEnumerator status;

    [HideInInspector]
    public bool skipAll;
    private bool skipCurrent;
    private bool confirmed;

    private Queue<TutorialParam[]> tutorialQueue;
    private IEnumerator currentTutorial;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        tutorialQueue = new Queue<TutorialParam[]>();
    }

    #region Instruction (top of screen)
    public void ShowInstruction(string content, params LocalizedParam[] param) {
        instructionText.SetText(content, param);
    }

    public void ClearInstruction() {
        instructionText.Clear();
    }
    #endregion

    #region Status (bottom of screen)
    /// <summary>
    /// Shows a tutorial text for a certain time.
    /// </summary>
    public void ShowStatus(string id, params LocalizedParam[] args) {
        if (status != null) {
            StopCoroutine(status);
        }

        status = ShowStatusHelper(2.0f, id, args);
        StartCoroutine(status);
    }

    /// <summary>
    /// Shows the status helper.
    /// </summary>
    /// <returns>The status helper.</returns>
    /// <param name="duration">duration of display.</param>
    IEnumerator ShowStatusHelper(float duration, string id, params LocalizedParam[] args) {
        statusText.SetText(id, args);
        yield return new WaitForSeconds(duration);
        statusText.Clear();

        status = null;
        yield return null;
    }
    #endregion

    #region Tutorial (overlay canvas)
    /// <summary>
    /// Shows the tutorial of one page. 
    /// Notice that this does NOT block.
    /// For blocking, refer to
    /// http://www.feelouttheform.net/unity3d-coroutine-synchronous/
    /// But notice that this won't work with ShowUntil as this equals a while(true)
    /// loop that will never end.
    /// </summary>
    /// <param name="param">parameter for tutorial.</param>
    /// <param name="preCallback">Callback function *before* any tutorial is shown.</param>
    public void ShowTutorial(TutorialParam param, System.Action preCallback = null) {
        TutorialParam[] groups = { param };
        ShowTutorial(groups, preCallback);
    }

    /// <summary>
    /// Shows the tutorial of several pages. A pre-callback function is available.
    /// Post-callback is available through the last TutorialParam's callback variable.
    /// Because the current implementation is asynchronous, if multiple calls to
    /// ShowTutorial() come in in short intervals, they might interfere with each
    /// other. To ensure tutorials goes one-by-one, use queue to cache future tutorials.
    /// </summary>
    /// <param name="groups">Groups.</param>
    public void ShowTutorial(TutorialParam[] groups, System.Action preCallback = null) {
        if (!skipAll) {
            if (currentTutorial == null) {
                currentTutorial = ShowTutorialHelper(groups, preCallback);
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
    private IEnumerator ShowTutorialHelper(TutorialParam[] groups, System.Action preCallback) {
        tutorialCanvas.SetActive(true);
        InputManager.Instance.menuOpened = true;
        if (preCallback != null) {
            preCallback();
        }
        foreach (TutorialParam group in groups) {
            canvasText.SetText(group.id, group.args);
            // The reason we don't use () => confirmed || skipCurrent || skipAll
            // is that *if* we do this we would see one frame of each remaining text
            // whichi is not desired. By setting confirmed to true in button callbacks
            // we can hide all remaining messages completely when the user chooses
            // to skip.
            yield return new WaitUntil(() => confirmed);
            if (group.callback != null) {
                group.callback();
            }
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

    /// <summary>
    /// Skips all tutorials.
    /// </summary>
    public void SkipAll() {
        controlButtons.SetActive(false);
        confirmationPanel.SetActive(true);
    }

    public void ConfirmSkipAll() {
        skipAll = true;
        confirmed = true;
        controlButtons.SetActive(true);
        confirmationPanel.SetActive(false);
    }

    public void CancelSkipAll() {
        controlButtons.SetActive(true);
        confirmationPanel.SetActive(false);
    }

    public void Next() {
        confirmed = true;
    }

    public void Skip() {
        skipCurrent = true;
        confirmed = true;
    }
    #endregion
}

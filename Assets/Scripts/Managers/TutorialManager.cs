using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private LocalizedText instructionText;
    [SerializeField] private LocalizedText statusText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private LocalizedText tutorialTitle;
    [SerializeField] private LocalizedText tutorialText;
    [SerializeField] private Renderer tutorialRenderer;
    [SerializeField] GameObject tutorialIcon;
    [SerializeField] TutorialPanel panelController;

    private IEnumerator status;
    private IEnumerator tutorial;
    private IEnumerator tutorialVisible;

    public bool SkipAll { get; set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
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

    #region Tutorial panel
    /// <summary> Displays a tutorial on the panel.</summary>
    /// <param name="param"> Contains the title and text for the tutorial.</param>
    /// <param name="trans"> Dictates where the panel will be at.</param>
    /// <param name="condition"> An optional condition to dicdate when to close the tutorial.
    /// Default is to wait 5 seconds.</param>
    /// <param name="preCallback"> An optional callback to be executed before the tutorial.</param>
    /// <param name="postCallback"> An optional callback to be executed after the tutorial.</param>
    /// <param name="mode"> What action should be taken if the tutorial goes out of sight.</param>
    public void ShowTutorial(TutorialParam param, Transform trans,
        Func<bool> condition = null, Action preCallback = null, Action postCallback = null,
        TutorialRemindMode mode = TutorialRemindMode.Icon) {
        if (tutorial != null) {
            ClearTutorial();
        }
        
        if (!SkipAll) {
            tutorial = ShowTutorialHelper(param, trans, condition, preCallback, postCallback);
            StartCoroutine(tutorial);
            if (mode == TutorialRemindMode.Icon) {
                tutorialVisible = Icon();
            } else if (mode == TutorialRemindMode.Follow) {
                tutorialVisible = Follow(trans);
            }

            if (tutorialVisible != null) {
                StartCoroutine(tutorialVisible);
            }
        }
    }

    private IEnumerator ShowTutorialHelper(TutorialParam param, Transform trans,
        Func<bool> condition, Action preCallback, Action postCallback) {
        preCallback?.Invoke();

        tutorialPanel.SetActive(true);
        tutorialPanel.transform.SetParent(trans, false);

        tutorialTitle.SetText(param.Title.Id, param.Title.Args);
        tutorialText.SetText(param.Contents.Id, param.Contents.Args);
        panelController.UpdatePanel();
        if (condition != null) {
            yield return new WaitUntil(condition);
        } else {
            yield return new WaitForSeconds(5);
        }

        postCallback?.Invoke();

        tutorialPanel.SetActive(false);
        tutorial = null;
        if (tutorialVisible != null) {
            StopCoroutine(tutorialVisible);
            tutorialVisible = null;
            tutorialIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Forces clear the tutorial. Does NOT invoke the post callback.
    /// You should use the "condition" to terminate the tutorial whenever you can.
    /// </summary>
    public void ClearTutorial() {
        if (tutorial != null) {
            tutorialPanel.SetActive(false);
            StopCoroutine(tutorial);
            tutorial = null;
            if (tutorialVisible != null) {
                StopCoroutine(tutorialVisible);
                tutorialIcon.SetActive(false);
            }
        }
    }

    /// <summary>
    /// If the tutorial panel is not visible, an icon will be shown on the screen
    /// to inform the user that there is a new tutorial available.
    /// To calculate which side the tutorial is at, we perform a cross product
    /// between the two vectors: camera-front and camera-panel. We then dot the result
    /// with the camera up vector and check its positivity.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Icon() {
        Transform camTransform = Camera.main.transform;
        RectTransform tutTransform = tutorialIcon.GetComponent<RectTransform>();
        Rect rect = tutorialIcon.transform.parent.GetComponent<Canvas>()
            .GetComponent<RectTransform>().rect;
        float mid = Mathf.Atan2(rect.width, rect.height);

        while (true) {
            if (tutorialPanel.activeSelf && !tutorialRenderer.isVisible) {
                tutorialIcon.SetActive(true);

                Vector3 normal = camTransform.forward;
                Vector3 c2o = tutorialPanel.transform.position - camTransform.position;
                Vector3 projection = Vector3.ProjectOnPlane(c2o, normal);
                float deg = Vector3.SignedAngle(camTransform.up, projection, normal);
                // Rotation
                tutTransform.localEulerAngles = new Vector3(0, 0, deg);

                // The following calculations are in radians
                deg *= Mathf.Deg2Rad;

                // Position
                float posDeg = Mathf.Abs(deg);
                if (posDeg > Mathf.PI / 2) {
                    posDeg = Mathf.PI - posDeg;
                }

                float dist = posDeg < mid
                    ? rect.height / 2 / Mathf.Cos(posDeg)
                    : rect.width / 2 / Mathf.Cos(Mathf.PI / 2 - posDeg);

                dist -= 100; // Leave some offset between icon and edge of screen

                tutTransform.localPosition = new Vector3(-Mathf.Sin(deg) * dist,
                    Mathf.Cos(deg) * dist, 0);
            } else {
                tutorialIcon.SetActive(false);
            }

            yield return null;
        }
    }

    /// <summary>
    /// If a tutorial panel is not visible, it will gradually fly into the sight of the camera.
    /// </summary>
    private IEnumerator Follow(Transform tutTransform) {
        Transform camTransform = Camera.main.transform;
        while (true) {
            if (tutorialPanel.activeSelf && !tutorialRenderer.isVisible) {
                yield return MoveCamera(camTransform, tutTransform);
            }
            
            yield return null;
        }
    }

    private IEnumerator MoveCamera(Transform camTransform, Transform tutTransform) {
        Vector3 target = camTransform.position + camTransform.forward * 0.5f;
        Vector3 currPos = tutTransform.position;
        while (Vector3.Distance(target, currPos) > 0.1f) {
            tutTransform.position = Vector3.Lerp(currPos, target, 0.03f);
            tutTransform.rotation = camTransform.rotation;
            yield return null;
            target = camTransform.position + camTransform.forward * 0.5f;
            currPos = tutTransform.position;
        }
    }
    #endregion
}
using System.Collections;
using UnityEngine;

public class StartPanelManager : MonoBehaviour {
    public static StartPanelManager Instance { get; private set; }

    [SerializeField] private GameObject startPanel;
    [SerializeField] private LocalizedText languageButtonText;

    private IEnumerator positionCoroutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        startPanel.SetActive(true);
        startPanel.transform.SetParent(Camera.main.transform);
        positionCoroutine = PositionPanel();
        StartCoroutine(positionCoroutine);
    }

    private IEnumerator PositionPanel() {
        Vector3 forward = new Vector3(0, -0.075f, 0);
        forward.z = Application.isEditor ? 0.2f : 0.5f;

        Vector3 rotation = Vector3.zero;
        while (true) {
            startPanel.transform.localPosition = forward;
            // TODO: temporarily disable rotation freeze
            // The current start panel has a y axis rotation of 90 degrees
            //rotation.y = Camera.main.transform.eulerAngles.y + 90;
            //if (!Application.isEditor) {
            //    startPanel.transform.eulerAngles = rotation;
            //}
            yield return null;
        }
    }

    public void Confirm() {
        if (Application.isEditor) {
            StageManager.Instance.ToggleStage(true);
            ArchetypeManager.Instance.LoadArchetypes();

            AppStateManager.Instance.currState = AppState.PlaceStage;
        } else {
            AppStateManager.Instance.currState = AppState.FindPlane;
            PlaneManager.Instance.BeginScan();
            TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        }
        startPanel.SetActive(false);
        StopCoroutine(positionCoroutine);
    }

    /// <summary>
    /// Changes the text for the language button.
    /// </summary>
    /// <param name="langID">Index for the language, defined in <see cref="Language"/>.</param>
    public void ToggleLanguage(int id) {
        languageButtonText.SetText("Buttons.Language",
            new LocalizedParam("General.Lang-" + ((Language)id).ToString(), true));
    }
}

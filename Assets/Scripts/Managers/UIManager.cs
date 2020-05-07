using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject confirmButton;

    public Transform interactionTutorialTransform;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        startCanvas.SetActive(true);
        InputManager.Instance.menuOpened = true;
    }

    public void SelectLanguage(int lang) {
        LocalizationManager.Instance.ChangeLanguage(lang);
        confirmButton.SetActive(true);
    }

    /// <summary>
    /// TODO: redundant code. Awaiting refactoring.
    /// </summary>
    public void ConfirmLanguage() {
#if UNITY_EDITOR
        StageManager.Instance.ToggleStage(true);
        ArchetypeManager.Instance.LoadArchetypes();

        AppStateManager.Instance.currState = AppState.PlaceStage;
#else
        AppStateManager.Instance.currState = AppState.FindPlane;
        PlaneManager.Instance.BeginScan();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
#endif
        startCanvas.SetActive(false);
        InputManager.Instance.menuOpened = false;
    }
}

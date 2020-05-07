using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    #region Welcome screen
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
        AppStateManager.Instance.currState = AppState.PickArchetype;

        StageManager.Instance.ToggleStage(true);
        ArchetypeManager.Instance.LoadArchetypes();
        TutorialManager.Instance.ClearInstruction();
        StageManager.Instance.HideStageObject();
        ArchetypeManager.Instance.SetIdlePose();

        // This will be the first time the user uses the interaction system.
        // So a tutorial is added here.
        TutorialParam content = new TutorialParam(
                "Tutorials.InteractionTitle", "Tutorials.InteractionText");
        TutorialManager.Instance.ShowTutorial(content,
            interactionTutorialTransform,
            () => ArchetypeManager.Instance.archetypeSelected);

        AppStateManager.Instance.currState = AppState.PickArchetype;
#else
        AppStateManager.Instance.currState = AppState.FindPlane;
        PlaneManager.Instance.BeginScan();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
#endif
        startCanvas.SetActive(false);
        InputManager.Instance.menuOpened = false;
    }
    #endregion
}

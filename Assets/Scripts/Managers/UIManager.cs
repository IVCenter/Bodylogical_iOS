using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject startPanel;

    public Transform interactionTutorialTransform;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        startPanel.SetActive(true);
        InputManager.Instance.menuOpened = true;
    }

    /// <summary>
    /// TODO: redundant code. Awaiting refactoring.
    /// </summary>
    public void Confirm() {
#if UNITY_EDITOR
        StageManager.Instance.ToggleStage(true);
        ArchetypeManager.Instance.LoadArchetypes();

        AppStateManager.Instance.currState = AppState.PlaceStage;
#else
        AppStateManager.Instance.currState = AppState.FindPlane;
        PlaneManager.Instance.BeginScan();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
#endif
        startPanel.SetActive(false);
        InputManager.Instance.menuOpened = false;
    }
}

using UnityEngine;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }

    [SerializeField] private float minScale, maxScale;

    [HideInInspector] public bool finding;

    public bool PlaneFound { get; private set; }

    public GameObject MainPlane { get; private set; }

    private bool isConfirming;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    // Update is called once per frame
    private void Update() {
        if (finding) {
            if (!PlaneFound) {
                float scale = GetComponent<FindLargestPlane>().LargestPlaneScale;
                DebugText.Instance.Log("plane scale is: " + scale);
                if (scale > maxScale) {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneGood");
                    isConfirming = true;
                } else if (scale > minScale) {
                    TutorialManager.Instance.ShowInstruction("Instructions.PlaneCont");
                }
            }

            if (isConfirming) {
                if (InputManager.Instance.TouchCount > 0) {
                    TutorialManager.Instance.ClearInstruction();
                    MainPlane = gameObject.GetComponent<FindLargestPlane>().Finish();
                    PlaneFound = true;
                    isConfirming = false;
                }
            }
        }
    }

    public void BeginScan() {
        GetComponent<FindLargestPlane>().Begin();
        finding = true;
    }

    public void RestartScan() {
        gameObject.GetComponent<FindLargestPlane>().Reset();
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        isConfirming = false;
        PlaneFound = false;
    }

    public void HideMainPlane() {
        MainPlane.SetActive(false);
    }
}

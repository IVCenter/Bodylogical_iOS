using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour {
    public static PlaneManager Instance { get; private set; }
    [SerializeField] private Transform freeTutorialTransform;
    public bool useImageTracking { get; private set; }
    public bool PlaneConfirmed { get; private set; }
    private bool planeFound;
    public List<GameObject> Planes { get; private set; }
    private PlaneFinder planeFinder;
    private ImageFinder imageFinder;

    /// <summary>
    /// Singleton set up.
    /// </summary>
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        planeFinder = GetComponent<PlaneFinder>();
        imageFinder = GetComponent<ImageFinder>();
    }

    private IEnumerator Scan() {
        TutorialManager.Instance.ShowInstruction("Instructions.PlaneFind");
        TutorialParam param = new TutorialParam("Tutorials.PlaneTitle", "Tutorials.PlaneText");
        TutorialManager.Instance.ShowTutorial(param, freeTutorialTransform, () => planeFound,
            mode: TutorialRemindMode.Follow);

        bool planeTutorialShown = false, imageTutorialShown = false;
        useImageTracking = false;
        while (!PlaneConfirmed) {
            if (imageFinder.image != null) { // A tracked image is found. Switch to image mode and stop plane tracking.
                useImageTracking = true;
                Planes = planeFinder.Finish();
                HidePlanes();
                planeFound = true;
                if (!imageTutorialShown) {
                    imageTutorialShown = true;
                    yield return null; // Let the previous tutorial finish
                    param = new TutorialParam("Tutorials.ImageConfirmTitle", "Tutorials.ImageConfirmText");
                    TutorialManager.Instance.ShowTutorial(param, freeTutorialTransform, () => PlaneConfirmed,
                        mode: TutorialRemindMode.Follow);
                }
            }

            if (!useImageTracking && planeFinder.planes.Count > 0) {
                planeFound = true;
                if (!planeTutorialShown) {
                    planeTutorialShown = true;
                    yield return null; // Let the previous tutorial finish
                    param = new TutorialParam("Tutorials.ConfirmTitle", "Tutorials.ConfirmText");
                    TutorialManager.Instance.ShowTutorial(param, freeTutorialTransform,
                        () => PlaneConfirmed || useImageTracking, mode: TutorialRemindMode.Follow);
                }
            }

            if (planeFound && InputManager.Instance.TouchCount > 0 && InputManager.Instance.TapCount > 1) {
                // Double tap
                TutorialManager.Instance.ClearInstruction();
                Planes = useImageTracking ? new List<GameObject> { imageFinder.Finish() } : planeFinder.Finish();
                PlaneConfirmed = true;
            }

            yield return null;
        }
    }

    public void BeginScan() {
        planeFinder.Begin();
        imageFinder.Begin();
        StartCoroutine(Scan());
    }
    

    public void RestartScan() {
        planeFinder.Resume();
        imageFinder.Resume();
        PlaneConfirmed = false;
        StartCoroutine(Scan());
    }

    public void HidePlanes() {
        if (Planes != null) {
            // If in debug/editor, MainPlane will be null.
            foreach (GameObject p in Planes) {
                p.SetActive(false);
            }
        }
    }
}
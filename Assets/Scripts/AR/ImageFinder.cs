using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Uses ARFoundation to find an image as an anchor to place the main stage.
/// </summary>
public class ImageFinder : MonoBehaviour {
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [HideInInspector] public List<ARTrackedImage> images;

    /// <summary>
    /// Initializes the finder.
    /// </summary>
    private void Start() {
        arTrackedImageManager.enabled = false;
        images = new List<ARTrackedImage>();
    }

    /// <summary>
    /// Begins the image scanning process.
    /// </summary>
    public void Begin() {
        SubscribeEvent();
    }

    /// <summary>
    /// Also stops image detection.
    /// </summary>
    /// <returns>A List of images that remain in the scene.</returns>
    public List<GameObject> Finish() {
        UnsubscribeEvent();

        List<GameObject> objs = new List<GameObject>();
        foreach (ARTrackedImage img in images) {
            objs.Add(img.gameObject);
        }

        return objs;
    }

    /// <summary>
    /// Displays all images and resumes plane detection.
    /// </summary>
    public void Resume() {
        foreach (ARTrackedImage img in arTrackedImageManager.trackables) {
            img.gameObject.SetActive(true);
        }

        SubscribeEvent();
    }

    /// <summary>
    /// Updates the images array.
    /// </summary>
    /// <param name="args">Arguments. We are interested in added, updated, and removed.</param>
    private void OnPlanesChanged(ARTrackedImagesChangedEventArgs args) {
        foreach (ARTrackedImage image in args.removed) {
            images.Remove(image);
        }

        foreach (ARTrackedImage image in args.added) {
            images.Add(image);
            DebugText.Instance.Log("ABCDE");
        }
    }

    private void SubscribeEvent() {
        arTrackedImageManager.trackedImagesChanged += OnPlanesChanged;
        arTrackedImageManager.enabled = true;
    }

    private void UnsubscribeEvent() {
        arTrackedImageManager.trackedImagesChanged -= OnPlanesChanged;
        arTrackedImageManager.enabled = false;
    }
}
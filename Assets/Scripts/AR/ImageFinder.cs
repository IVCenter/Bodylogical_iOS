using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Uses ARFoundation to find an image as an anchor to place the main stage.
/// </summary>
public class ImageFinder : MonoBehaviour {
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    // Right now, only keep track of one image
    [HideInInspector] public ARTrackedImage image;

    /// <summary>
    /// Initializes the finder.
    /// </summary>
    private void Start() {
        arTrackedImageManager.enabled = false;
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
    public GameObject Finish() {
        UnsubscribeEvent();
        return image.gameObject;
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
    private void OnImagesChanged(ARTrackedImagesChangedEventArgs args) {
        foreach (ARTrackedImage img in args.removed) {
            if (img == image) {
                image = null;
            }
        }

        foreach (ARTrackedImage img in args.added) {
            image = img;
        }
    }

    private void SubscribeEvent() {
        arTrackedImageManager.trackedImagesChanged += OnImagesChanged;
        arTrackedImageManager.enabled = true;
    }

    private void UnsubscribeEvent() {
        arTrackedImageManager.trackedImagesChanged -= OnImagesChanged;
        arTrackedImageManager.enabled = false;
    }
}
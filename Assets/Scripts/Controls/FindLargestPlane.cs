using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FindLargestPlane : MonoBehaviour {
    public ARPlaneManager arPlaneManager;

    private ARPlane largestPlane;

    public float LargestPlaneScale => GetPlaneSize(largestPlane);

    /// <summary>
    /// Subscribes to planesChanged event.
    /// </summary>
    void Start() {
        arPlaneManager.planesChanged += OnPlanesChanged;
        arPlaneManager.enabled = false;
    }

    public void Begin() {
        arPlaneManager.enabled = true;
    }

    public GameObject Finish() {
        foreach (ARPlane plane in arPlaneManager.trackables) {
            if (plane != largestPlane) {
                plane.gameObject.SetActive(false);
            }
        }
        arPlaneManager.planesChanged -= OnPlanesChanged;
        arPlaneManager.enabled = false;
        return largestPlane.gameObject;
    }

    public void Reset() {
        largestPlane.gameObject.SetActive(false);
        arPlaneManager.planesChanged += OnPlanesChanged;
        arPlaneManager.enabled = true;
    }

    /// <summary>
    /// Finds out the largest plane detected.
    /// First, check if the largest plane recorded in this instance has been removed.
    /// Then, browse through added and updated to find the largest plane.
    /// </summary>
    /// <param name="args">Arguments. We are interested in added, updated, and removed.</param>
    private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
        foreach (ARPlane plane in args.removed) {
            if (plane == largestPlane) {
                largestPlane = null;
            }
        }

        foreach (ARPlane plane in args.added) {
            if (GetPlaneSize(plane) > LargestPlaneScale) {
                largestPlane = plane;
            }
        }

        foreach (ARPlane plane in args.updated) {
            if (GetPlaneSize(plane) > LargestPlaneScale) {
                largestPlane = plane;
            }
        }
    }

    private float GetPlaneSize(ARPlane plane) {
        return plane == null ? 0 : plane.size.x * plane.size.y;
    }
}

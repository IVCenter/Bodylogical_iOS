using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Uses ARFoundation to find a plane to place the main stage.
/// </summary>
public class PlaneFinder : MonoBehaviour {
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private float minSize, maxSize;
    [HideInInspector] public List<ARPlane> planes;

    /// <summary>
    /// Initializes the finder.
    /// </summary>
    private void Start() {
        arPlaneManager.enabled = false;
        planes = new List<ARPlane>();
    }

    /// <summary>
    /// Begins the plane scanning process.
    /// </summary>
    public void Begin() {
        SubscribeEvent();
    }

    /// <summary>
    /// Hides all smaller planes and shows all large ones.
    /// Also stops plane detection.
    /// </summary>
    /// <returns>A List of planes that remain in the scene.</returns>
    public List<GameObject> Finish() {
        // Hide all planes first
        foreach (ARPlane plane in arPlaneManager.trackables) {
            plane.gameObject.SetActive(false);
        }
        
        UnsubscribeEvent();

        List<GameObject> objs = new List<GameObject>();
        foreach (ARPlane p in planes) {
            p.gameObject.SetActive(true);
            objs.Add(p.gameObject);
        }
        return objs;
    }

    /// <summary>
    /// Displays all planes and resumes plane detection.
    /// </summary>
    public void Resume() {
        foreach (ARPlane p in arPlaneManager.trackables) {
            p.gameObject.SetActive(true);
        }

        SubscribeEvent();
    }

    /// <summary>
    /// Finds out the largest plane detected.
    /// First, check if the largest plane recorded in this instance has been removed.
    /// Then, browse through added and updated to find the largest plane.
    /// </summary>
    /// <param name="args">Arguments. We are interested in added, updated, and removed.</param>
    private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
        foreach (ARPlane plane in args.removed) {
            if (planes.Contains(plane)) {
                planes.Remove(plane);
            }
        }

        foreach (ARPlane plane in args.added) {
            if (CheckPlaneSize(plane)) {
                planes.Add(plane);
            }
        }

        foreach (ARPlane plane in args.updated) {
            bool exist = planes.Contains(plane);
            if (CheckPlaneSize(plane) && !exist) {
                planes.Add(plane);
            } else if (!CheckPlaneSize(plane) && exist) {
                planes.Remove(plane);
            }
        }
    }

    private bool CheckPlaneSize(ARPlane plane) {
        if (plane == null) {
            return false;
        }

        float min = Mathf.Min(plane.size.x, plane.size.y);
        float max = Mathf.Max(plane.size.x, plane.size.y);
        return min >= minSize && max >= maxSize;
    }

    private void SubscribeEvent() {
        arPlaneManager.planesChanged += OnPlanesChanged;
        arPlaneManager.enabled = true;
    }

    private void UnsubscribeEvent() {
        arPlaneManager.planesChanged -= OnPlanesChanged;
        arPlaneManager.enabled = false;
    }
}

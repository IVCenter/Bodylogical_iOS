using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneFinder : MonoBehaviour {
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private float minSize, maxSize;
    [HideInInspector] public List<ARPlane> planes;

    /// <summary>
    /// Subscribes to planesChanged event.
    /// </summary>
    void Start() {
        arPlaneManager.planesChanged += OnPlanesChanged;
        arPlaneManager.enabled = false;
        planes = new List<ARPlane>();
    }

    public void Begin() {
        arPlaneManager.enabled = true;
    }

    public List<GameObject> Finish() {
        foreach (ARPlane plane in arPlaneManager.trackables) {
            if (!Exists(plane)) {
                plane.gameObject.SetActive(false);
            }
        }
        arPlaneManager.planesChanged -= OnPlanesChanged;
        arPlaneManager.enabled = false;

        List<GameObject> objs = new List<GameObject>();
        foreach (ARPlane p in planes) {
            objs.Add(p.gameObject);
        }
        return objs;
    }

    public void Reset() {
        foreach (ARPlane p in arPlaneManager.trackables) {
            p.gameObject.SetActive(true);
        }

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
            if (Exists(plane)) {
                planes.Remove(plane);
            }
        }

        foreach (ARPlane plane in args.added) {
            if (CheckPlaneSize(plane)) {
                planes.Add(plane);
            }
        }

        foreach (ARPlane plane in args.updated) {
            bool exist = Exists(plane);
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

    private bool Exists(ARPlane plane) {
        foreach (ARPlane p in planes) {
            if (p == plane) {
                return true;
            }
        }
        return false;
    }
}

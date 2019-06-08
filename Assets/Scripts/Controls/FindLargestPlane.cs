using System.Collections.Generic;
using Collections.Hybrid.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class FindLargestPlane : MonoBehaviour {
    public GameObject planePrefab;

    private MeshCollider meshCollider; //declared to avoid code stripping of class
    private MeshFilter meshFilter; //declared to avoid code stripping of class
    private string largestPlaneID = "";
    private LinkedListDictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;
    private GameObject fixedLargestPlane;
    private bool isProcessing;

    private void Awake() {
        planeAnchorMap = new LinkedListDictionary<string, ARPlaneAnchorGameObject>();
        SubscribeEvents();
        isProcessing = true;
    }

    #region Events
    public void SubscribeEvents() {
        UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
    }

    public void UnsubscribeEvents() {
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent -= RemoveAnchor;
    }

    public void AddAnchor(ARPlaneAnchor arPlaneAnchor) {
        GameObject go = CreatePlane(arPlaneAnchor);
        go.AddComponent<DontDestroyOnLoad>();  //this is so these GOs persist across scene loads

        ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject {
            planeAnchor = arPlaneAnchor,
            gameObject = go
        };

        planeAnchorMap.Add(arPlaneAnchor.identifier, arpag);
    }

    public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor) {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier)) {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            Destroy(arpag.gameObject);
            planeAnchorMap.Remove(arPlaneAnchor.identifier);
        }
    }

    public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor) {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier)) {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            UpdatePlaneWithAnchorTransform(arpag.gameObject, arPlaneAnchor);
            arpag.planeAnchor = arPlaneAnchor;
            planeAnchorMap[arPlaneAnchor.identifier] = arpag;
        } else {
            AddAnchor(arPlaneAnchor);
        }
    }
    #endregion


    public void Destroy() {
        foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors()) {
            Destroy(arpag.gameObject);
        }

        planeAnchorMap.Clear();
        UnsubscribeEvents();
    }

    public LinkedList<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors() {
        return planeAnchorMap.Values;
    }

    public float GetCurrentLargestPlaneScale() {
        if (largestPlaneID != "") {
            MeshFilter mf = planeAnchorMap[largestPlaneID].gameObject.GetComponentInChildren<MeshFilter>();
            float x_scale = mf.gameObject.transform.localScale.x;
            float z_scale = mf.gameObject.transform.localScale.z;
            float total_scale = x_scale * z_scale;

            return total_scale;
        }

        return 0;
    }

    // should return the reference to the largest plane in the scene and delete all else 
    // deregister event
    public GameObject FinishProcess() {
        if (!isProcessing || largestPlaneID == "") {
            DebugText.Instance.Log("Cannot Finish Process");

            // either return null or the stored largest plane
            return fixedLargestPlane;
        }

        GameObject largestPlane = planeAnchorMap[largestPlaneID].gameObject;

        foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors()) {
            if (arpag.planeAnchor.identifier != largestPlaneID) {
                Destroy(arpag.gameObject);
            }
        }

        fixedLargestPlane = largestPlane;
        planeAnchorMap.Clear();
        UnsubscribeEvents();
        isProcessing = false;
        return largestPlane;
    }

    public void RestartProcess() {
        if (isProcessing) {
            DebugText.Instance.Log("Cannot Restart Process");
            return;
        }

        if (fixedLargestPlane != null) {
            Destroy(fixedLargestPlane);
        }

        // This would reset the entire session, but would break the lighting.
        //ARKitWorldTrackingSessionConfiguration sessionConfig = new ARKitWorldTrackingSessionConfiguration(UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection.Horizontal);
        //UnityARSessionNativeInterface.GetARSessionNativeInterface().RunWithConfigAndOptions(sessionConfig, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking);

        SubscribeEvents();
        isProcessing = true;
        largestPlaneID = "";
    }

    private GameObject CreatePlane(ARPlaneAnchor arPlaneAnchor) {
        GameObject plane;
        if (planePrefab != null) {
            plane = Instantiate(planePrefab);
        } else {
            plane = new GameObject(); //put in a blank gameObject to get at least a transform to manipulate
        }

        plane.name = arPlaneAnchor.identifier;

        ARKitPlaneMeshRender apmr = plane.GetComponent<ARKitPlaneMeshRender>();
        if (apmr != null) {
            apmr.InitiliazeMesh(arPlaneAnchor);
        }

        return UpdatePlaneWithAnchorTransform(plane, arPlaneAnchor);
    }

    private GameObject UpdatePlaneWithAnchorTransform(GameObject plane, ARPlaneAnchor arPlaneAnchor) {
        //do coordinate conversion from ARKit to Unity
        plane.transform.position = UnityARMatrixOps.GetPosition(arPlaneAnchor.transform);
        plane.transform.rotation = UnityARMatrixOps.GetRotation(arPlaneAnchor.transform);

        ARKitPlaneMeshRender apmr = plane.GetComponent<ARKitPlaneMeshRender>();
        if (apmr != null) {
            apmr.UpdateMesh(arPlaneAnchor);
        }

        MeshFilter mf = plane.GetComponentInChildren<MeshFilter>();

        if (mf != null) {
            if (apmr == null) {
                //since our plane mesh is actually 10mx10m in the world, we scale it here by 0.1f
                mf.gameObject.transform.localScale = new Vector3(arPlaneAnchor.extent.x * 0.1f, arPlaneAnchor.extent.y * 0.1f, arPlaneAnchor.extent.z * 0.1f);

                //convert our center position to unity coords
                mf.gameObject.transform.localPosition = new Vector3(arPlaneAnchor.center.x, arPlaneAnchor.center.y, -arPlaneAnchor.center.z);
            }

        }

        CalculateLargestPlane();

        return plane;
    }


    private void CalculateLargestPlane() {
        float xScale, zScale, totalScale = 0;

        foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors()) {
            MeshFilter mf = arpag.gameObject.GetComponentInChildren<MeshFilter>();
            if (mf.gameObject.transform.localScale.x * mf.gameObject.transform.localScale.z > totalScale) {
                xScale = mf.gameObject.transform.localScale.x;
                zScale = mf.gameObject.transform.localScale.z;
                totalScale = xScale * zScale;
                largestPlaneID = arpag.planeAnchor.identifier;
            }
        }
    }
}

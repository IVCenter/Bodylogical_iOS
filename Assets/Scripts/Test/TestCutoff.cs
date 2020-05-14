using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCutoff : MonoBehaviour {
    [SerializeField] private GameObject plane;

    [SerializeField] private GameObject crossSectionModel;
    [SerializeField] private GameObject wireframeModel;

    private Material crossSectionMat;
    private Material wireframeMat;
    private Transform planeTransform;

    private IEnumerator planeDown;

    // Start is called before the first frame update
    void Start() {
        planeTransform = plane.transform;

        wireframeMat = wireframeModel.GetComponent<Renderer>().material;
        wireframeMat.SetVector("_V_WIRE_DynamicMaskWorldNormal", Vector3.up);

        crossSectionMat = crossSectionModel.GetComponent<Renderer>().material;
        crossSectionMat.SetVector("_PlaneNormal", Vector3.up);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && planeDown == null) {
            planeDown = PlaneDown();
            StartCoroutine(planeDown);
        }
    }

    private IEnumerator PlaneDown() {
        Debug.Log("Start");

        Vector3 position = new Vector3(0, 2, 0);
        // Bring plane up
        planeTransform.position = position;
        wireframeMat.SetVector("_V_WIRE_DynamicMaskWorldPos", position);
        crossSectionMat.SetVector("_PlanePosition", position);
        yield return null;

        for (int i = 0; i < 1000; i++) {
            position.y -= 0.002f;
            planeTransform.position = position;
            wireframeMat.SetVector("_V_WIRE_DynamicMaskWorldPos", position);
            crossSectionMat.SetVector("_PlanePosition", position);
            yield return null;
        }

        Debug.Log("Done Descending");

        yield return new WaitForSeconds(1);

        for (int i = 0; i < 1000; i++) {
            position.y += 0.002f;
            planeTransform.position = position;
            wireframeMat.SetVector("_V_WIRE_DynamicMaskWorldPos", position);
            crossSectionMat.SetVector("_PlanePosition", position);
            yield return null;
        }

        Debug.Log("Done Ascednging");

        planeDown = null;
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriusVisualizer : MonoBehaviour {
    public Material goodMat, intermediateMat, badMat;
    public GameObject heartIndicator, liverIndicator, kidneyIndicator;

    public void Visualize() {
        heartIndicator.GetComponent<MeshRenderer>().material = SetMaterial();
        liverIndicator.GetComponent<MeshRenderer>().material = SetMaterial();
        kidneyIndicator.GetComponent<MeshRenderer>().material = SetMaterial();
    }

    /// <summary>
    /// FOR TESTING PURPOSES ONLY.
    /// TODO: to be removed by real data
    /// </summary>
    private Material SetMaterial() {
        int point = Random.Range(0, 10);
        if (point < 4) {
            return badMat;
        } else if (point < 7) {
            return intermediateMat;
        } else {
            return goodMat;
        }
    }
}

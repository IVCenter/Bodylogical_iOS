using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransformParenting : MonoBehaviour {
    public GameObject modelTemplate, humanModel;

    // Start is called before the first frame update
    void Start() {
        GameObject modelParent = Instantiate(modelTemplate);

        GameObject human = Instantiate(humanModel);

        Transform modelTransform = modelParent.transform.Find("ModelParent/model");
        human.transform.SetParent(modelTransform, false);
        //human.transform.localPosition = new Vector3(0, 0, 0);
    }
}

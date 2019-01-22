using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransformFind : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        print(transform.childCount);
        Transform title = transform.Search("Title");
        if (title == null) {
            print("NULL");
        } else {
            print("FOUND");
        }
    }

    // Update is called once per frame
    void Update() {

    }
}

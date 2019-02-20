using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiate : MonoBehaviour {
    public GameObject prefab1, prefab2;
    // Start is called before the first frame update
    void Start() {
        print(Instantiate(prefab1));
        print(Instantiate(prefab2));
    }
}

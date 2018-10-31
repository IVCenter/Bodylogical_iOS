using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour {

    public static HumanManager Instance;

    public GameObject humanModel;

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

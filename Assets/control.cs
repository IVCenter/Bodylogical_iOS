using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control : MonoBehaviour {

    public GameObject prefabToUse;
    public ServerData server;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space"))
        {
            // create prefab in current space
            prefabToUse.SetActive(true);
            GameObject poi = Instantiate(prefabToUse);
            poi.transform.position = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
            prefabToUse.SetActive(false);
            server.SendTransform(poi, "misc");
            Debug.Log("Dropped an egg");
        }
	}
}
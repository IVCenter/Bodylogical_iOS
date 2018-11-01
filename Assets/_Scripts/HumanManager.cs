using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collections.Hybrid.Generic;

public class HumanManager : MonoBehaviour {

    public static HumanManager Instance;

    public GameObject male_prefab;
    public GameObject female_prefab;

    public GameObject humanModel;

    public LinkedListDictionary<string, Archetype> archetypeMap;

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }
    }

    public bool CreateArchitype(string ProfileName, string Name = "Bob", float Weight = 160, string sex = "male"){

        Archetype go = new Archetype(ProfileName, Name, Weight, sex);
        go.InstantiateModel(male_prefab, female_prefab);

        if(!go.CreateModel(sex)){
            return false;
        }

        archetypeMap.Add(ProfileName, go);
        return true;
    }


    // Use this for initialization
    void Start () {
        archetypeMap = new LinkedListDictionary<string, Archetype>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

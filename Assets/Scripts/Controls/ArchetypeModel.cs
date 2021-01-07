using UnityEngine;

public abstract class ArchetypeModel {
    public Archetype ArchetypeData { get; }
    public GameObject Model { get; }
    public Material Mat { get; }
    public Animator ArchetypeAnimator { get; }
    
    protected ArchetypeModel(GameObject prefab, Archetype archetypeData, Transform parent) {
        ArchetypeData = archetypeData;

        Model = Object.Instantiate(prefab, parent, false);
        Transform modelTransform = Model.transform.Find("model");

        GameObject figure = Object.Instantiate(Resources.Load<GameObject>($"Prefabs/{archetypeData.modelString}"),
            modelTransform, false);
        Mat = figure.transform.GetChild(0).GetComponent<Renderer>().material;
        ArchetypeAnimator = figure.transform.GetComponent<Animator>();
    }
    
    public void Dispose() {
        Object.Destroy(Model);
    }
}
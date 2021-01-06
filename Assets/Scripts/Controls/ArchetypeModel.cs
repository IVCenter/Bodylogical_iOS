using UnityEngine;

public abstract class ArchetypeModel {
    public Archetype ArchetypeData { get; protected set; }
    public GameObject Model { get; protected set; }
    public Material Mat { get; protected set; }
    public Animator ArchetypeAnimator { get; protected set; }
    
    protected ArchetypeModel(Archetype archetypeData, Transform parent) {
        ArchetypeData = archetypeData;

        Model = Object.Instantiate(ArchetypeManager.Instance.displayerPrefab, parent, false);
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
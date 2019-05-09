using UnityEngine;

/// <summary>
/// Source: https://docs.unity3d.com/Manual/BlendShapes.html
/// </summary>
public class TestBlendShape : MonoBehaviour {
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshCollider meshCollider;

    [Range(0, 100)]
    public float blendProgress;

    void Awake() {
        if (skinnedMeshRenderer == null) {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }
        if (meshCollider == null) {
            meshCollider = GetComponent<MeshCollider>();
        }
    }

    //void Update() {
    //    if (blendOne < 100f) {
    //        skinnedMeshRenderer.SetBlendShapeWeight(0, blendOne);
    //        blendOne += blendSpeed;
    //    }
    //}

    private void OnValidate() {
        skinnedMeshRenderer.SetBlendShapeWeight(0, blendProgress);
        Mesh bakeMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakeMesh);
        meshCollider.sharedMesh = bakeMesh;
    }
}

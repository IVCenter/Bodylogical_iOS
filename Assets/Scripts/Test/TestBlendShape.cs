using UnityEngine;

/// <summary>
/// Source: https://docs.unity3d.com/Manual/BlendShapes.html
/// </summary>
public class TestBlendShape : MonoBehaviour {
    public SkinnedMeshRenderer skinnedMeshRenderer;
    [Range(0, 100)]
    public float blendProgress;

    //private readonly float blendSpeed = 1f;

    void Awake() {
        if (skinnedMeshRenderer == null) {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
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
    }
}

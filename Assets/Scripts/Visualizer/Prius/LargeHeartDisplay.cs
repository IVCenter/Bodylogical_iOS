using UnityEngine;

public class LargeHeartDisplay : OrganDisplay {
    public GameObject heart;
    public SkinnedMeshRenderer vesselRenderer;

    public override void DisplayOrgan(int score) {
        vesselRenderer.SetBlendShapeWeight(0, 100 - score);
        // Attempt to make collisions work for 
        Mesh bakeMesh = new Mesh();
        vesselRenderer.BakeMesh(bakeMesh);
        vesselRenderer.GetComponent<MeshCollider>().sharedMesh = bakeMesh;
    }
}

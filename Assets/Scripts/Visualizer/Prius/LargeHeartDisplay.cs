using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeHeartDisplay : OrganDisplay {
    public GameObject heart;
    public Animator heartAnimator;
    public SkinnedMeshRenderer vesselRenderer;

    public override void DisplayOrgan(int score) {
        // calculate animation speed: from 0.6 (score 100) to 1 (score 0)
        heartAnimator.speed = 1.0f - score * 0.004f;
        vesselRenderer.SetBlendShapeWeight(0, 100 - score);
        // Attempt to make collisions work for 
        Mesh bakeMesh = new Mesh();
        vesselRenderer.BakeMesh(bakeMesh);
        vesselRenderer.GetComponent<MeshCollider>().sharedMesh = bakeMesh;
    }
}

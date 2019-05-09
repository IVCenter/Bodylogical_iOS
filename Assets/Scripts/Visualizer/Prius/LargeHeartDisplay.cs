using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeHeartDisplay : OrganDisplay {
    public GameObject heart;
    private Animator HeartAnimator { get { return heart.transform.GetChild(0).GetChild(0).GetComponent<Animator>(); } }
    private SkinnedMeshRenderer VesselRenderer { get { return heart.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>(); } }

    public override void DisplayOrgan(int score) {
        // calculate animation speed: from 0.6 (score 100) to 1 (score 0)
        HeartAnimator.speed = 1.0f - score * 0.004f;
        VesselRenderer.SetBlendShapeWeight(0, 100 - score);
        Mesh bakeMesh = new Mesh();
        VesselRenderer.BakeMesh(bakeMesh);
        VesselRenderer.GetComponent<MeshCollider>().sharedMesh = bakeMesh;
    }
}

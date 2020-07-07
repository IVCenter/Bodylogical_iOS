using System.Collections;
using UnityEngine;

public class DataFlowParticle : MonoBehaviour {
    [SerializeField] private Vector3[] positions;
    [SerializeField] private float speed;
    
    private void Start() {
        StartCoroutine(Travel());
    }

    private IEnumerator Travel() {
        while (true) {
            for (int i = 0; i < positions.Length - 1; i++) {
                transform.position = positions[i];
                yield return null;
                
                float traveledDist = 0;
                float totalDist = Vector3.Distance(positions[i], positions[i + 1]);
                Vector3 dir = Vector3.Normalize(positions[i + 1] - positions[i]);
                while (traveledDist < totalDist) {
                    transform.position += dir * speed;
                    traveledDist += speed;
                    yield return null;
                }
            }
        }
    }
}

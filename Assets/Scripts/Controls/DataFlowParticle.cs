using System.Collections;
using UnityEngine;

/// <summary>
/// Particle effect that would travel along a route.
/// </summary>
public class DataFlowParticle : MonoBehaviour {
    [SerializeField] private Vector3[] positions;
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem trail;
    [SerializeField]private Color[] colors;
    
    private ParticleSystem.MainModule module;
    private IEnumerator travel;

    private void Start() {
        module = trail.main;
        travel = Travel();
        StartCoroutine(travel);
    }

    private IEnumerator Travel() {
        HealthStatus status = HealthUtil.CalculateStatus(HealthLoader.Instance
            .ChoiceDataDictionary[TimeProgressManager.Instance.Path].CalculateHealth(
                TimeProgressManager.Instance.YearValue,
                ArchetypeManager.Instance.Selected.ArchetypeData.gender));
        module.startColor = colors[(int) status];

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

            // Move the particle to the beginning of the route
            gameObject.SetActive(false);
            transform.position = positions[0];
            yield return new WaitForSeconds(5);
        }
    }
}
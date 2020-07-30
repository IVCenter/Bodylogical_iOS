using System.Collections;
using UnityEngine;

/// <summary>
/// Particle effect that would travel along a route.
/// </summary>
public class DataFlowParticle : MonoBehaviour {
    [SerializeField] private Transform[] route;
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private ParticleSystem glow;
    
    public Color BaseColor { get; set; }
    private ParticleSystem.MainModule trailModule;
    private ParticleSystem.MainModule glowModule;
    private IEnumerator travel;
    private float RealSpeed => speed * transform.lossyScale.x;

    /// <summary>
    /// During initialization, move the particle to the beginning of the route to prevent "flashing" effects.
    /// </summary>
    public void Initialize() {
        trailModule = trail.main;
        glowModule = glow.main;
        transform.position = route[0].position;
        BaseColor = trailModule.startColor.color; // Give a basic color for debugging
    }

    public void Visualize() {
        if (travel == null) {
            SetActive(true);
            travel = Travel();
            StartCoroutine(travel);
        }
    }

    public void Stop() {
        if (travel != null) {
            StopCoroutine(travel);
            travel = null;
            SetActive(false);
        }
    }

    private IEnumerator Travel() {
        transform.position = route[0].position;

        while (true) {
            SetParticleColor();
            for (int i = 0; i < route.Length - 1; i++) {
                float traveledDist = 0;
                float totalDist = Vector3.Distance(route[i].position, route[i + 1].position);
                Vector3 dir = Vector3.Normalize(route[i + 1].position - route[i].position);
                while (traveledDist < totalDist) {
                    transform.position += dir * RealSpeed;
                    traveledDist += RealSpeed;
                    yield return null;
                }
            }

            // Stop for a few seconds
            yield return new WaitForSeconds(2);

            // Move the particle to the beginning of the route
            // There is a bug in Unity that when a gameObject is disabled, the coroutine will automatically stop.
            // Therefore, disable all the children instead.
            SetActive(false);
            transform.position = route[0].position;
            yield return new WaitForSeconds(2);
            SetActive(true);
        }
    }

    private void SetActive(bool on) {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(on);
        }
    }

    public void SetParticleColor() {
        Color deltaColor = new Color(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        trailModule.startColor = BaseColor + deltaColor;
        glowModule.startColor = BaseColor;
    }
}
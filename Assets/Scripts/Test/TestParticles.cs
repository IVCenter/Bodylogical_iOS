using UnityEngine;

public class TestParticles : MonoBehaviour {
    private enum Mode {
        MouseFollow,
        Trail
    }

    [SerializeField] private Mode mode;
    [SerializeField] private DataFlowParticle[] particles;
    [SerializeField] private float radius;
    [SerializeField] private float speed;

    private float frameSpeed;
    private void Start() {
        Application.targetFrameRate = 60;
        frameSpeed = speed * Time.deltaTime;
        if (mode == Mode.Trail) {
            foreach (DataFlowParticle particle in particles) {
                particle.Visualize();
            }
        }
    }
    private void Update() {
        if (mode == Mode.MouseFollow) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = radius;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 dir = Vector3.Normalize(pos - transform.position);
            foreach (DataFlowParticle particle in particles) {
                particle.transform.position += dir * frameSpeed;
            }
        }
    }
}
using UnityEngine;

public class TestParticles : MonoBehaviour {
    [SerializeField] private float radius;
    [SerializeField] private float speed;

    private float frameSpeed;
    private void Start() {
        Application.targetFrameRate = 60;
        frameSpeed = speed * Time.deltaTime;
    }
    private void Update() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = radius;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 dir = Vector3.Normalize(pos - transform.position);
        transform.position += dir * frameSpeed;
    }
}
using System.Collections;
using UnityEngine;

public class ArchetypeModel : MonoBehaviour {
    private const float Epsilon = 0.001f;
    private static readonly int Walk = Animator.StringToHash("Walk");

    public Archetype ArchetypeData { get; set; }
    public DetailPanel panel;

    private Material material;
    private Animator animator;

    public Material Mat =>
        material
            ? material
            : material = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Renderer>().material;

    public Animator Anim => animator ? animator : animator = transform.GetChild(1).GetChild(0).GetComponent<Animator>();

    public IEnumerator MoveTo(Vector3 endPos) {
        Transform trans = transform;
        Vector3 forward = trans.forward;

        // Calculate if the archetype needs to travel, and if so, which direction to rotate
        Vector3 startPos = trans.position;
        Vector3 direction = startPos - endPos;
        direction.y = 0; // Ignore elevation
        direction = Vector3.Normalize(direction);

        if (Vector3.Distance(startPos, endPos) < Epsilon) {
            // epsilon value, no need to move
            yield break;
        }

        Vector3 rotation = trans.localEulerAngles;
        float startAngle = rotation.y;
        float targetAngle = Vector3.SignedAngle(forward, direction, Vector3.up);
        float progress;

        // Rotate archetype
        for (progress = 0; progress < 1; progress += 0.02f) {
            rotation.y = startAngle + Mathf.SmoothStep(0, targetAngle, progress);
            trans.localEulerAngles = rotation;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Move archetype
        Anim.SetBool(Walk, true);
        for (progress = 0; progress < 1; progress += 0.01f) {
            Vector3 newPos = new Vector3(
                Mathf.SmoothStep(startPos.x, endPos.x, progress),
                Mathf.SmoothStep(startPos.y, endPos.y, progress),
                Mathf.SmoothStep(startPos.z, endPos.z, progress)
            );
            trans.position = newPos;
            yield return null;
        }

        trans.position = endPos;
        Anim.SetBool(Walk, false);
        yield return new WaitForSeconds(0.5f);

        // Rotate back
        for (progress = 0; progress < 1; progress += 0.02f) {
            rotation.y = startAngle + Mathf.SmoothStep(targetAngle, 0, progress);
            trans.localEulerAngles = rotation;
            yield return null;
        }

        yield return null;
    }
}
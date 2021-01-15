using System.Collections;
using UnityEngine;

public class BackwardsProps : PropAnimation {
    // TODO: random props
    // [System.Serializable]
    // private class Prop {
    //     public GameObject obj;
    //     public float rotateY;
    // }

    /// <summary>
    /// The props should be children of the script, otherwise the clipping planes might have problems.
    /// </summary>
    [SerializeField] private Transform[] props;
    [SerializeField] private float minBound = -1.5f;
    [SerializeField] private float maxBound = 1.5f;

    private float cutoff = 0.7f;
    // Shader property hashes
    private static readonly int plane1Position = Shader.PropertyToID("_Plane1Position");
    private static readonly int plane2Position = Shader.PropertyToID("_Plane2Position");
    
    /// <summary>
    /// Initialize the clipping plane boundaries
    /// </summary>
    private void Start() {
        Vector3 minPlane = transform.TransformPoint(new Vector3(0, 0, minBound * cutoff));
        Vector3 maxPlane = transform.TransformPoint(new Vector3(0, 0, maxBound * cutoff));
        foreach (Transform prop in props) {
            foreach (Material m in prop.GetComponent<Renderer>().materials) {
                m.SetVector(plane1Position, minPlane);
                m.SetVector(plane2Position, maxPlane);
            }
        }
    }
    
    public override IEnumerator Animate() {
        Vector3[] propPositions = new Vector3[props.Length];
        for (int i = 0; i < propPositions.Length; i++) {
            propPositions[i] = props[i].localPosition;
        }

        while (true) {
            for (int i = 0; i < propPositions.Length; i++) {
                propPositions[i].z += Speed * Time.deltaTime;
                if (propPositions[i].z > maxBound) {
                    propPositions[i].z = minBound;
                }

                props[i].transform.localPosition = propPositions[i];
            }

            yield return null;
        }
    }

    public override void Toggle(bool on) {
        foreach (Transform tr in props) {
            tr.gameObject.SetActive(on);
        }
    }
}
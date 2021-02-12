using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardsProps : PropAnimation {
    /// <summary>
    /// The props should be children of the script, otherwise the clipping planes might have problems.
    /// </summary>
    [SerializeField] private PropTemplate[] templates;

    // The prop will travel from minBound/minPlane to maxBound/maxPlane
    [SerializeField] private float minBound = -1.5f;
    [SerializeField] private float maxBound = 1.5f;

    private class Prop {
        public GameObject obj;
        public float length;

        public Transform Trans => obj.transform;

        public Prop(PropTemplate template) {
            obj = Instantiate(template.gameObject);
            length = template.length;
        }
    }

    private List<Prop> curr = new List<Prop>();
    private Prop next;

    private const float MinCutoff = 0.65f;
    private const float MaxCutoff = 0.75f;

    private Vector3 MinPlane => transform.TransformPoint(new Vector3(0, 0, minBound * MinCutoff));
    private Vector3 MaxPlane => transform.TransformPoint(new Vector3(0, 0, maxBound * MaxCutoff));

    // Shader property hashes
    private static readonly int Plane1Position = Shader.PropertyToID("_Plane1Position");
    private static readonly int Plane2Position = Shader.PropertyToID("_Plane2Position");

    public override IEnumerator Animate() {
        // Generate first prop
        curr.Add(GenerateProp());
        next = GenerateProp();

        while (true) {
            for (int i = 0; i < curr.Count; i++) {
                Vector3 vec = curr[i].Trans.localPosition;
                vec.z += Speed * Time.deltaTime;
                curr[i].Trans.localPosition = vec;

                if (vec.z > maxBound) {
                    // Destroy this prop
                    Destroy(curr[i].obj);
                    curr.RemoveAt(i);
                    i--;
                }

                if (i == curr.Count - 1) {
                    // Check if there is a large enough space between the last prop and the next prop
                    // (with some randomization factor)
                    if (i == -1 || vec.z - minBound > (curr[i].length / 2 + next.length / 2) * Random.Range(3f, 6f)) {
                        curr.Add(next);
                        next = GenerateProp();
                    }
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Generates a random prop from the list. The prop will be placed behind the min plane.
    /// </summary>
    /// <returns></returns>
    private Prop GenerateProp() {
        int index = Random.Range(0, templates.Length);
        Prop prop = new Prop(templates[index]);
        prop.obj.transform.SetParent(transform, false);
        foreach (Material m in prop.obj.GetComponent<Renderer>().materials) {
            m.SetVector(Plane1Position, MinPlane);
            m.SetVector(Plane2Position, MaxPlane);
        }

        Vector3 pos = prop.Trans.localPosition;
        pos.z = minBound;
        prop.Trans.localPosition = pos;

        return prop;
    }

    public override void Toggle(bool on) {
        foreach (Prop prop in curr) {
            prop.obj.SetActive(on);
        }
    }

    public void ResetProps() {
        foreach (Prop prop in curr) {
            Destroy(prop.obj);
        }

        curr.Clear();
        
        Destroy(next.obj);
        next = null;
    }
}
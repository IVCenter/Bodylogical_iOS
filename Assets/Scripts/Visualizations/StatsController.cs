using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StatsController : MonoBehaviour {
    [SerializeField] private Color[] colors;
    [SerializeField] private Transform front;
    [SerializeField] private float pathLength;
    
    private ArchetypePerformer performer;
    private Mesh mesh;
    private Vector3 originalPos;

    public void Initialize(ArchetypePerformer archetypePerformer) {
        performer = archetypePerformer;
        originalPos = performer.transform.position;

        mesh = new Mesh {name = "Stats"};
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public IEnumerator Toggle(bool on) {
        gameObject.SetActive(true);
        // TODO: need some redesign to fit all the data
        // performer.panel.Toggle(on);
        // performer.panel.UpdateStats();

        performer.icon.SetActive(false);
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        if (on) {
            // Move the avatar to front of road
            yield return performer.MoveTo(front.position, false);
            mr.enabled = true;
        } else {
            // Move the avatar to middle of road
            yield return performer.MoveTo(originalPos);
            gameObject.SetActive(false);
        }

        performer.icon.SetActive(true);

        yield return null;
    }

    /// <summary>
    /// Creates the color road indicating the archetype's health.s
    /// </summary>
    public void BuildStats() {
        int numHealths = performer.ArchetypeHealth.Count;
        Vector3[] vPos = new Vector3[numHealths * 2];
        Color[] vColor = new Color[numHealths * 2];
        int[] vTri = new int[(numHealths - 1) * 6];
        for (int i = 0; i < numHealths; i++) {
            float z = pathLength * i / numHealths;
            vPos[i * 2] = new Vector3(-1, 0, z);
            vPos[i * 2 + 1] = new Vector3(1, 0, z);

            int score = performer.ArchetypeHealth.CalculateHealth(i, performer.ArchetypeData.gender);
            Color color = colors[score * (colors.Length - 1) / 100];
            vColor[i * 2] = color;
            vColor[i * 2 + 1] = color;

            if (i < numHealths - 1) {
                vTri[i * 6 + 0] = i * 2;
                vTri[i * 6 + 1] = i * 2 + 2;
                vTri[i * 6 + 2] = i * 2 + 1;
                vTri[i * 6 + 3] = i * 2 + 1;
                vTri[i * 6 + 4] = i * 2 + 2;
                vTri[i * 6 + 5] = i * 2 + 3;
            }
        }

        mesh.vertices = vPos;
        mesh.colors = vColor;
        mesh.triangles = vTri;
        mesh.RecalculateNormals();
    }
}
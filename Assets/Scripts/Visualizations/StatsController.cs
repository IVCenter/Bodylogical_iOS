using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StatsController : MonoBehaviour {
    [SerializeField] private Color[] colors;
    [SerializeField] private Transform front;

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
        // TODO
        //performer.panel.Toggle(on);

        performer.icon.SetActive(false);
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        if (on) {
            // Move the avatar to front of road
            yield return performer.MoveTo(front.position);
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
        int years = performer.ArchetypeHealth.Count;
        Vector3[] vPos = new Vector3[years * 2];
        Color[] vColor = new Color[years * 2];
        int[] vTri = new int[(years - 1) * 6];
        for (int i = 0; i < years; i++) {
            vPos[i * 2] = new Vector3(-1, 0, i);
            vPos[i * 2 + 1] = new Vector3(1, 0, i);

            int score = performer.ArchetypeHealth.CalculateHealth(i, performer.ArchetypeData.gender);
            Color color = colors[score * (colors.Length - 1) / 100];
            vColor[i * 2] = color;
            vColor[i * 2 + 1] = color;

            if (i < years - 1) {
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
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TestMesh : MonoBehaviour {
    [SerializeField] private Color[] colors;
    private Mesh mesh;
    
    private void Start() {
        mesh = new Mesh {name = "Road"};
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vPos = new Vector3[colors.Length * 2];
        Color[] vColor = new Color[colors.Length * 2];
        int[] vTri = new int[(colors.Length - 1) * 6];
        for (int i = 0; i < colors.Length; i++) {
            vPos[i * 2] = new Vector3(-1, 0, i);
            vPos[i * 2 + 1] = new Vector3(1, 0, i);
            
            vColor[i * 2] = colors[i];
            vColor[i * 2 + 1] = colors[i];

            if (i < colors.Length - 1) {
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
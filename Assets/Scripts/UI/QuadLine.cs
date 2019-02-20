using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadLine : MonoBehaviour {
    public Material redmat;
    public Material yellowmat;
    public Material greenmat;

    private bool isLineCreated;

    #region Unity Routines
    /// <summary>
    /// Debug
    /// </summary>
    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            CreateAllLines();
        }
    }
    #endregion

    public void ResetLines() {
        foreach(Transform ribbon in transform) {
            Destroy(ribbon.gameObject);
        }

    }

    public void CreateAllLines() {
        if (isLineCreated) {
            return;
        }

        isLineCreated = true;

        foreach (GameObject section in YearPanelManager.Instance.yearPanels[0].sections) {
            CreateLinesForAllPanels(section.name);
        }
    }

    private void CreateLinesForAllPanels(string panelName) {
        int len = YearPanelManager.Instance.yearPanels.Length;
        GameObject[] nothing = new GameObject[len];
        GameObject[] mini = new GameObject[len];
        GameObject[] rec = new GameObject[len];

        // for test
        for (int i = 0; i < len; i++) {
            nothing[i] = YearPanelManager.Instance.yearPanels[i].transform.Search(panelName).Search("Pointers").Search("Nothing").gameObject;
            mini[i] = YearPanelManager.Instance.yearPanels[i].transform.Search(panelName).Search("Pointers").Search("Minimum").gameObject;
            rec[i] = YearPanelManager.Instance.yearPanels[i].transform.Search(panelName).Search("Pointers").Search("Recommended").gameObject;
        }

        ConstructFullQLine(nothing, redmat);
        ConstructFullQLine(mini, yellowmat);
        ConstructFullQLine(rec, greenmat);
    }

    private void ConstructFullQLine(GameObject[] panelLists, Material mat) {
        for (int i = 0; i < panelLists.Length - 1; i++) {
            ConstructQLineBetween(panelLists[i], panelLists[i + 1], mat);
        }
    }

    private void ConstructQLineBetween(GameObject panel1, GameObject panel2, Material mat) {
        // create a new quad line
        GameObject qline = new GameObject("QuadLine");
        qline.transform.parent = transform;

        MeshFilter mf = qline.AddComponent<MeshFilter>();
        MeshRenderer mr = qline.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mr.material = mat;

        Vector3[] vertices = GetVertices(panel1, panel2);

        mesh.vertices = vertices;

        int[] tri = new int[12];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        tri[6] = 2;
        tri[7] = 0;
        tri[8] = 1;

        tri[9] = 3;
        tri[10] = 2;
        tri[11] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;
    }

    private Vector3[] GetVertices(GameObject prevYearPanel, GameObject currYearPanel) {
        Vector3 bottom_left = prevYearPanel.transform.position - new Vector3(0, 0.005804f, 0);
        Vector3 bottom_right = currYearPanel.transform.position - new Vector3(0, 0.005804f, 0);
        Vector3 top_left = prevYearPanel.transform.position + new Vector3(0, 0.005804f, 0);
        Vector3 top_right = currYearPanel.transform.position + new Vector3(0, 0.005804f, 0);

        Vector3[] vertices = new Vector3[4];

        vertices[0] = bottom_left;
        vertices[1] = bottom_right;
        vertices[2] = top_left;
        vertices[3] = top_right;

        return vertices;
    }
}
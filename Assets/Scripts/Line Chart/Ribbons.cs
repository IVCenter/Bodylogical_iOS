using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ribbons that span across different biometric panels.
/// </summary>
public class Ribbons : MonoBehaviour {
    private bool lineCreated;
    private readonly List<GameObject> ribbons = new List<GameObject>();

    /// <summary>
    /// Creates all lines.
    /// </summary>
    public void CreateAllLines(Color pointerColor, Material ribbonMaterial) {
        if (!lineCreated) {
            lineCreated = true;

            foreach (GameObject section in LineChartManager.Instance.yearPanels[0].sections) {
                CreateLinesForPanel(section.name, pointerColor, ribbonMaterial);
            }
        }
    }

    /// <summary>
    /// Creates the lines for one single metric.
    /// </summary>
    /// <param name="panelName">Panel name.</param>
    /// <param name="pointerColor">Color of the ribbon.</param>
    /// <param name="ribbonMaterial">Material of the ribbons.</param>
    private void CreateLinesForPanel(string panelName, Color pointerColor, Material ribbonMaterial) {
        int len = LineChartManager.Instance.yearPanels.Length;

        for (int i = 0; i < len - 1; i++) {
            GameObject panel = LineChartManager.Instance.yearPanels[i].transform.
                Search(panelName).Search("Pointer").gameObject;
            // set color of pointer
            panel.GetComponent<Image>().color = pointerColor;

            GameObject nextPanel = LineChartManager.Instance.yearPanels[i + 1].transform.
                Search(panelName).Search("Pointer").gameObject;

            ConstructQLineBetween(panel, nextPanel, ribbonMaterial);
        }

        // Set color of last pointer
        LineChartManager.Instance.yearPanels[len - 1].transform.Search(panelName).
            Search("Pointer").GetComponent<Image>().color = pointerColor;
    }

    /// <summary>
    /// Draws a ribbon between two panels.
    /// </summary>
    /// <param name="panel1">First panel to connect to.</param>
    /// <param name="panel2">Second panel to connect to.</param>
    /// <param name="mat">Material of ribbon.</param>
    private void ConstructQLineBetween(GameObject panel1, GameObject panel2, Material mat) {
        // create a new quad line
        GameObject qline = new GameObject("QuadLine");
        qline.transform.parent = panel1.transform.parent.parent.parent;
        ribbons.Add(qline);

        MeshFilter mf = qline.AddComponent<MeshFilter>();
        MeshRenderer mr = qline.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mr.material = mat;

        mesh.vertices = new Vector3[] {
            panel1.transform.position - new Vector3(0, 0.005804f, 0),
            panel2.transform.position - new Vector3(0, 0.005804f, 0),
            panel1.transform.position + new Vector3(0, 0.005804f, 0),
            panel2.transform.position + new Vector3(0, 0.005804f, 0)
        };
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1, 2, 0, 1, 3, 2, 1 };
        mesh.normals = new Vector3[] { 
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
    }

    /// <summary>
    /// Removes all ribbons.
    /// </summary>
    public void ResetLines() {
        if (ribbons != null) {
            foreach (GameObject ribbon in ribbons) {
                Destroy(ribbon);
            }
            ribbons.Clear();
        }
        lineCreated = false;
    }

    /// <summary>
    /// Shows/hides the ribbons.
    /// </summary>
    /// <param name="on">If true, display the ribbons.</param>
    public void ToggleRibbons(bool on) {
        foreach (GameObject ribbon in ribbons) {
            ribbon.SetActive(on);
        }
    }
}
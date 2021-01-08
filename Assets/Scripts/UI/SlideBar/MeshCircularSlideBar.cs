using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Same as Circular slide bar, but implemented in 3D with Unity meshes.
/// </summary>
public class MeshCircularSlideBar : SlideBarPointer {
    /// <summary>
    /// Intervals of radians. The smaller, the more precise.
    /// </summary>
    [SerializeField] private float precision = 0.1f;

    [SerializeField] private float outerRadius = 1;
    [SerializeField] private float innerRadius = 0.7f;
    [SerializeField] private float thickness = 0.5f;

    private enum Direction {
        Left,
        Right,
        Top,
        Bottom
    };

    /// <summary>
    /// Starting point of status.
    /// </summary>
    [SerializeField] private Direction direction = Direction.Right;

    [SerializeField] private Color[] colors;

    /// <summary>
    /// Intervals for different colors (in increasing order).
    /// interval: 0 -- i[0] -- i[1] -- i[2] -- ...
    /// color:     c[0] -- c[1] -- c[2] -- ...
    /// Please be sure that the last interval value is 100.
    /// </summary>
    [SerializeField] private int[] intervals;

    private Mesh mesh;

    private Mesh SlideBarMesh {
        get {
            if (mesh == null) {
                mesh = new Mesh {
                    name = "Status"
                };
                GetComponent<MeshFilter>().mesh = mesh;
                material = new Material(Shader.Find("Standard"));
                GetComponent<MeshRenderer>().sharedMaterial = material;
            }

            return mesh;
        }
    }
    private Material material;

    private static readonly Dictionary<Direction, float> offsets = new Dictionary<Direction, float>() {
        {Direction.Right, 0}, {Direction.Top, Mathf.PI / 2},
        {Direction.Left, Mathf.PI}, {Direction.Bottom, Mathf.PI * 3 / 2}
    };

    public override void SetProgress(int progress) {
        this.progress = progress;
        BuildModel();
        // Assign color
        for (int i = 0; i < intervals.Length; i++) {
            if (progress <= intervals[i]) {
                material.color = colors[i];
                break;
            }
        }
    }

    private void BuildModel() {
        SlideBarMesh.Clear();

        float degree = progress / 50f * Mathf.PI;

        // Vertices
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        float startDeg = offsets[direction];
        float endDeg = degree + offsets[direction];

        // First face
        vertices.Add(new Vector3(innerRadius * Mathf.Cos(startDeg),
            innerRadius * Mathf.Sin(startDeg),
            thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(startDeg),
            outerRadius * Mathf.Sin(startDeg),
            thickness / 2));
        vertices.Add(new Vector3(innerRadius * Mathf.Cos(startDeg),
            innerRadius * Mathf.Sin(startDeg),
            -thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(startDeg),
            outerRadius * Mathf.Sin(startDeg),
            -thickness / 2));
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(2);

        // Top and bottom faces
        float deg;
        int i;

        for (deg = startDeg; deg < endDeg; deg += precision) {
            vertices.Add(new Vector3(innerRadius * Mathf.Cos(deg),
                innerRadius * Mathf.Sin(deg),
                thickness / 2));
            vertices.Add(new Vector3(outerRadius * Mathf.Cos(deg),
                outerRadius * Mathf.Sin(deg),
                thickness / 2));
            vertices.Add(new Vector3(innerRadius * Mathf.Cos(deg),
                innerRadius * Mathf.Sin(deg),
                -thickness / 2));
            vertices.Add(new Vector3(outerRadius * Mathf.Cos(deg),
                outerRadius * Mathf.Sin(deg),
                -thickness / 2));
        }

        vertices.Add(new Vector3(innerRadius * Mathf.Cos(endDeg),
            innerRadius * Mathf.Sin(endDeg),
            thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(endDeg),
            outerRadius * Mathf.Sin(endDeg),
            thickness / 2));
        vertices.Add(new Vector3(innerRadius * Mathf.Cos(endDeg),
            innerRadius * Mathf.Sin(endDeg),
            -thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(endDeg),
            outerRadius * Mathf.Sin(endDeg),
            -thickness / 2));

        for (i = 4; i + 4 < vertices.Count; i += 4) {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 4);
            triangles.Add(i + 5);
            triangles.Add(i + 4);
            triangles.Add(i + 1);

            triangles.Add(i + 3);
            triangles.Add(i + 2);
            triangles.Add(i + 7);
            triangles.Add(i + 6);
            triangles.Add(i + 7);
            triangles.Add(i + 2);
        }

        // Side faces
        i = vertices.Count;
        for (deg = startDeg; deg < endDeg; deg += precision) {
            vertices.Add(new Vector3(innerRadius * Mathf.Cos(deg),
                innerRadius * Mathf.Sin(deg),
                thickness / 2));
            vertices.Add(new Vector3(outerRadius * Mathf.Cos(deg),
                outerRadius * Mathf.Sin(deg),
                thickness / 2));
            vertices.Add(new Vector3(innerRadius * Mathf.Cos(deg),
                innerRadius * Mathf.Sin(deg),
                -thickness / 2));
            vertices.Add(new Vector3(outerRadius * Mathf.Cos(deg),
                outerRadius * Mathf.Sin(deg),
                -thickness / 2));
        }

        vertices.Add(new Vector3(innerRadius * Mathf.Cos(endDeg),
            innerRadius * Mathf.Sin(endDeg),
            thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(endDeg),
            outerRadius * Mathf.Sin(endDeg),
            thickness / 2));
        vertices.Add(new Vector3(innerRadius * Mathf.Cos(endDeg),
            innerRadius * Mathf.Sin(endDeg),
            -thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(endDeg),
            outerRadius * Mathf.Sin(endDeg),
            -thickness / 2));

        for (; i + 4 < vertices.Count; i += 4) {
            triangles.Add(i + 6);
            triangles.Add(i + 2);
            triangles.Add(i + 4);
            triangles.Add(i);
            triangles.Add(i + 4);
            triangles.Add(i + 2);

            triangles.Add(i + 3);
            triangles.Add(i + 7);
            triangles.Add(i + 1);
            triangles.Add(i + 5);
            triangles.Add(i + 1);
            triangles.Add(i + 7);
        }

        // Last face
        i = vertices.Count;
        vertices.Add(new Vector3(innerRadius * Mathf.Cos(endDeg),
            innerRadius * Mathf.Sin(endDeg),
            thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(endDeg),
            outerRadius * Mathf.Sin(endDeg),
            thickness / 2));
        vertices.Add(new Vector3(innerRadius * Mathf.Cos(endDeg),
            innerRadius * Mathf.Sin(endDeg),
            -thickness / 2));
        vertices.Add(new Vector3(outerRadius * Mathf.Cos(endDeg),
            outerRadius * Mathf.Sin(endDeg),
            -thickness / 2));
        triangles.Add(i - 2);
        triangles.Add(i - 4);
        triangles.Add(i - 1);
        triangles.Add(i - 3);
        triangles.Add(i - 1);
        triangles.Add(i - 4);

        SlideBarMesh.vertices = vertices.ToArray();
        SlideBarMesh.triangles = triangles.ToArray();
        SlideBarMesh.RecalculateNormals();
    }
}
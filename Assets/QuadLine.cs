using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadLine : MonoBehaviour {

    private float width;
    private float height;

    public Material redmat;
    public Material yellowmat;
    public Material greenmat;

    private bool isLineCreated;
    private string[] biometric_names = { "Overall Panel", "Body Fat Panel", "BMI Panel", "HbA1c Panel", "LDL Panel", "Blood Pressure Panel"};


    private void Awake()
    {
        isLineCreated = false;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A)){
            CreateAllLines();
        }
	}

    public void CreateAllLines(){

        if (isLineCreated)
        {
            return;
        }

        isLineCreated = true;

        foreach (string name in biometric_names){
            CreateLinesForAllPanels(name);
        }
    }

    public void CreateLinesForAllPanels(string biometric_name){
    

        GameObject[] nothing = new GameObject[5];
        GameObject[] mini = new GameObject[5];
        GameObject[] rec = new GameObject[5];

        // for test

        nothing[0] = YearPanelManager.Instance.year0panel.transform.Search(biometric_name).Search("Pointers").Search("Nothing").gameObject;
        nothing[1] = YearPanelManager.Instance.year1panel.transform.Search(biometric_name).Search("Pointers").Search("Nothing").gameObject;
        nothing[2] = YearPanelManager.Instance.year2panel.transform.Search(biometric_name).Search("Pointers").Search("Nothing").gameObject;
        nothing[3] = YearPanelManager.Instance.year3panel.transform.Search(biometric_name).Search("Pointers").Search("Nothing").gameObject;
        nothing[4] = YearPanelManager.Instance.year4panel.transform.Search(biometric_name).Search("Pointers").Search("Nothing").gameObject;

        mini[0] = YearPanelManager.Instance.year0panel.transform.Search(biometric_name).Search("Pointers").Search("Minimum").gameObject;
        mini[1] = YearPanelManager.Instance.year1panel.transform.Search(biometric_name).Search("Pointers").Search("Minimum").gameObject;
        mini[2] = YearPanelManager.Instance.year2panel.transform.Search(biometric_name).Search("Pointers").Search("Minimum").gameObject;
        mini[3] = YearPanelManager.Instance.year3panel.transform.Search(biometric_name).Search("Pointers").Search("Minimum").gameObject;
        mini[4] = YearPanelManager.Instance.year4panel.transform.Search(biometric_name).Search("Pointers").Search("Minimum").gameObject;

        rec[0] = YearPanelManager.Instance.year0panel.transform.Search(biometric_name).Search("Pointers").Search("Recommended").gameObject;
        rec[1] = YearPanelManager.Instance.year1panel.transform.Search(biometric_name).Search("Pointers").Search("Recommended").gameObject;
        rec[2] = YearPanelManager.Instance.year2panel.transform.Search(biometric_name).Search("Pointers").Search("Recommended").gameObject;
        rec[3] = YearPanelManager.Instance.year3panel.transform.Search(biometric_name).Search("Pointers").Search("Recommended").gameObject;
        rec[4] = YearPanelManager.Instance.year4panel.transform.Search(biometric_name).Search("Pointers").Search("Recommended").gameObject;

        ConstructFullQLine(nothing, redmat);
        ConstructFullQLine(mini, yellowmat);
        ConstructFullQLine(rec, greenmat);

    }

    private void ConstructFullQLine(GameObject[] panelLists, Material mat){

        for (int i = 0; i < panelLists.Length - 1; i++){
            ConstructQLineBetween(panelLists[i], panelLists[i + 1], mat);
        }
    }

    private void ConstructQLineBetween(GameObject panel1, GameObject panel2, Material mat){

        // create a new quad line
        GameObject qline = new GameObject("QuadLine");
        qline.transform.parent = gameObject.transform;

        MeshFilter mf = qline.AddComponent<MeshFilter>();
        MeshRenderer mr = qline.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mr.material = mat;

        Vector3[] vertices = get4vertices(panel1, panel2);

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

    public Vector3[] get4vertices(GameObject prevYearPanel, GameObject currYearPanel){

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

    //var mf: MeshFilter = GetComponent.<MeshFilter>();
    //var mesh = new Mesh();
    //mf.mesh = mesh;
    
    //var vertices: Vector3[] = new Vector3[4];
    
    //vertices[0] = new Vector3(0, 0, 0);
    //vertices[1] = new Vector3(width, 0, 0);
    //vertices[2] = new Vector3(0, height, 0);
    //vertices[3] = new Vector3(width, height, 0);

    //mesh.vertices = vertices;
    
    //var tri: int[] = new int[6];

    //tri[0] = 0;
    //tri[1] = 2;
    //tri[2] = 1;
    
    //tri[3] = 2;
    //tri[4] = 3;
    //tri[5] = 1;
    
    //mesh.triangles = tri;
    
    //var normals: Vector3[] = new Vector3[4];
    
    //normals[0] = -Vector3.forward;
    //normals[1] = -Vector3.forward;
    //normals[2] = -Vector3.forward;
    //normals[3] = -Vector3.forward;
    
    //mesh.normals = normals;
    
    //var uv: Vector2[] = new Vector2[4];

    //uv[0] = new Vector2(0, 0);
    //uv[1] = new Vector2(1, 0);
    //uv[2] = new Vector2(0, 1);
    //uv[3] = new Vector2(1, 1);

    //mesh.uv = uv;
}

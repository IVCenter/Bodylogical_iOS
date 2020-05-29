using System.Collections.Generic;
using UnityEngine;

public class DisplayInternals : MonoBehaviour {
    public float radius = 0.4f;
    [SerializeField] private GameObject internals;
    [SerializeField] private GameObject smallParts; // There are no "large parts" in this version
    public float attenuation = 0.8f;

    private Material archetypeMat;
    private float archetypeStartAlpha;
    private Material planeMat;
    private float planeStartAlpha;


    [SerializeField] private List<GameObject> boxes;
    [SerializeField] private List<GameObject> texts;
    private List<Material> boxMaterials;
    private List<float> boxAlphas;

    private void Start() {
        GetComponent<SphereCollider>().radius = radius;
        // Start() will be called when the game object is enabled.
        // At this time, the archetype will already be selected.
        archetypeMat = ArchetypeManager.Instance.ModelMaterial;
        archetypeStartAlpha = archetypeMat.GetFloat("_AlphaScale");

        planeMat = internals.transform.Find("Plane").GetComponent<Renderer>().material;
        planeStartAlpha = planeMat.color.a;

        boxes = internals.transform.SearchAllWithName("Box");
        texts = internals.transform.SearchAllWithName("Side", true);
        texts.AddRange(internals.transform.SearchAllWithName("Header"));

        boxMaterials = new List<Material>(boxes.Count);
        boxAlphas = new List<float>(boxes.Count);
        foreach (GameObject box in boxes) {
            Material mat = box.GetComponent<Renderer>().material;
            boxMaterials.Add(mat);
            boxAlphas.Add(mat.color.a);
        }

        foreach (GameObject text in texts) {
            text.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.name.Contains("Camera")) {
            internals.SetActive(true);
            float distance = Vector3.Distance(transform.position, other.transform.position);
            float percent = distance / radius;
            percent *= attenuation;

            for (int i = 0; i < boxes.Count; i++) {
                Color boxColor = boxMaterials[i].color;
                boxColor.a = boxAlphas[i] * (1 - percent);
                boxMaterials[i].color = boxColor;

                // Box wireframe color alpha default to 1
                Color wireColor = boxMaterials[i].GetColor("_V_WIRE_Color");
                wireColor.a = (1 - percent);
                boxMaterials[i].SetColor("_V_WIRE_Color", wireColor);
            }

            Color planeColor = planeMat.color;
            planeColor.a = planeStartAlpha * (1 - percent);
            planeMat.color = planeColor;

            if (percent < 0.6f) { // Close enough, display text
                foreach (GameObject text in texts) {
                    text.SetActive(true);
                }
                smallParts.SetActive(false);
                ArchetypeManager.Instance.SelectedModel.SetActive(false);
            } else {
                foreach (GameObject text in texts) {
                    text.SetActive(false);
                }
                ArchetypeManager.Instance.SelectedModel.SetActive(true);
                smallParts.SetActive(true);
                archetypeMat.SetFloat("_AlphaScale", archetypeStartAlpha * percent);
            }

        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name.Contains("Camera")) {
            internals.SetActive(false);
            archetypeMat.SetFloat("_AlphaScale", archetypeStartAlpha);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class DisplayInternals : MonoBehaviour {
    [SerializeField] private GameObject internals;
    [SerializeField] private GameObject smallParts; // There are no "large parts" in this version
    [SerializeField] private float attenuation = 0.9f;
    [SerializeField] private float cutoff = 0.8f;

    private Material archetypeMat;
    private Material planeMat;
    private float planeStartAlpha;


    private List<GameObject> boxes;
    private List<GameObject> texts;
    private List<Material> boxMaterials;
    private List<float> boxAlphas;

    private float radius;
    private bool avatarHidden;

    private void Start() {
        radius = GetComponent<SphereCollider>().radius;
        // Start() will be called when the game object is enabled.
        // At this time, the archetype will already be selected.
        archetypeMat = ArchetypeManager.Instance.ModelMaterial;
    }

    private void OnTriggerStay(Collider other) {
        if (other.name.Contains("Camera")) {
            internals.SetActive(true);
            float distance = Vector3.Distance(transform.position, other.transform.position);
            float percent = distance / radius * attenuation;

            bool newAvatarHidden = percent <= cutoff;

            if (avatarHidden && newAvatarHidden) {
                return; // still within cutoff range, don't do anything
            }

            if (newAvatarHidden) {
                // avatarHidden is false, just got in range
                // Display text and set to original transparency
                foreach (GameObject text in texts) {
                    text.SetActive(true);
                }
                smallParts.SetActive(false);
                ArchetypeManager.Instance.SelectedModel.SetActive(false);

                for (int i = 0; i < boxes.Count; i++) {
                    Color boxColor = boxMaterials[i].color;
                    boxColor.a = boxAlphas[i];
                    boxMaterials[i].color = boxColor;

                    // Box wireframe color alpha default to 1
                    Color wireColor = boxMaterials[i].GetColor("_V_WIRE_Color");
                    wireColor.a = 1;
                    boxMaterials[i].SetColor("_V_WIRE_Color", wireColor);
                }

                Color planeColor = planeMat.color;
                planeColor.a = planeStartAlpha;
                planeMat.color = planeColor;
            } else if (avatarHidden) {
                // newAvatarHidden is false, just got out of range
                // Hide text and reset transparency
                foreach (GameObject text in texts) {
                    text.SetActive(false);
                }
                ArchetypeManager.Instance.SelectedModel.SetActive(true);
                smallParts.SetActive(true);
                archetypeMat.SetFloat("_AlphaScale", percent);
            } else {
                // Adjust transparency
                for (int i = 0; i < boxes.Count; i++) {
                    Color boxColor = boxMaterials[i].color;
                    boxColor.a = boxAlphas[i] * (1 - percent);
                    boxMaterials[i].color = boxColor;

                    // Box wireframe color alpha default to 1
                    Color wireColor = boxMaterials[i].GetColor("_V_WIRE_Color");
                    wireColor.a = 1 - percent;
                    boxMaterials[i].SetColor("_V_WIRE_Color", wireColor);
                }

                Color planeColor = planeMat.color;
                planeColor.a = planeStartAlpha * (1 - percent);
                planeMat.color = planeColor;
            }

            avatarHidden = newAvatarHidden;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name.Contains("Camera")) {
            internals.SetActive(false);
            archetypeMat.SetFloat("_AlphaScale", 1);
        }
    }

    /// <summary>
    /// Set up the materials and alpha values.
    /// </summary>
    public void Initialize() {
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

        Reset();
    }

    /// <summary>
    /// After stage transition, all canvases will be set to active. They need
    /// to be reset to false.
    /// </summary>
    public void Reset() {
        foreach (GameObject text in texts) {
            text.SetActive(false);
        }
    }
}

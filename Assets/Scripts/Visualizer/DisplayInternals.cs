using System.Collections.Generic;
using UnityEngine;

public class DisplayInternals : MonoBehaviour {
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject internals;
    [SerializeField] private float cutoff = 0.95f;

    /// <summary>
    /// Color library used for internal visualization.
    /// </summary>
    [SerializeField] private ColorLibrary colorLibrary;

    private DataFlowParticle[] groundParticles;
    private DataFlowParticle[] internalsParticles;

    private Material archetypeMat;
    private Material planeMat;
    private float planeStartAlpha;

    private List<GameObject> boxes;
    private List<GameObject> texts;
    private List<Material> boxMaterials;
    private List<float> boxAlphas;

    private float radius;

    public bool AvatarHidden { get; private set; }

    // Shader property hashes 
    private static readonly int VWireColor = Shader.PropertyToID("_V_WIRE_Color");
    private static readonly int AlphaScale = Shader.PropertyToID("_AlphaScale");

    // Tutorial related variables
    [SerializeField] private Transform internalTutorialTransform;
    private bool tutorialShown;

    private void Start() {
        radius = GetComponent<SphereCollider>().radius;
        // Initialize the particles
        groundParticles = ground.GetComponentsInChildren<DataFlowParticle>();
        foreach (DataFlowParticle particle in groundParticles) {
            particle.Initialize();
        }

        internalsParticles = internals.GetComponentsInChildren<DataFlowParticle>();
        foreach (DataFlowParticle particle in internalsParticles) {
            particle.Initialize();
        }

        // Start() will be called when the game object is enabled.
        // At this time, the archetype will already be selected.
        archetypeMat = ArchetypeManager.Instance.Selected.Mat;

        foreach (DataFlowParticle particle in groundParticles) {
            particle.Visualize();
        }
    }

    /// <summary>
    /// This script will be attached objects with layer "Bounding Sphere",
    /// which would only collide with the layer "User", where the camera is in.
    /// Therefore, no checks are needed for the other collider.
    /// </summary>
    private void OnTriggerStay(Collider other) {
        internals.SetActive(true);
        float distance = Vector3.Distance(transform.position, other.transform.position);
        float percent = distance / radius;

        bool newAvatarHidden = percent <= cutoff;

        if (AvatarHidden && newAvatarHidden) {
            return; // still within cutoff range, don't do anything
        }

        if (newAvatarHidden) {
            // avatarHidden is false, just got in range
            // Display text and set to original transparency

            foreach (GameObject text in texts) {
                text.SetActive(true);
            }

            ArchetypeManager.Instance.Selected.Model.SetActive(false);

            for (int i = 0; i < boxes.Count; i++) {
                Color boxColor = boxMaterials[i].color;
                boxColor.a = boxAlphas[i];
                boxMaterials[i].color = boxColor;

                // Box wireframe color alpha default to 1
                Color wireColor = boxMaterials[i].GetColor(VWireColor);
                wireColor.a = 1;
                boxMaterials[i].SetColor(VWireColor, wireColor);
            }

            Color planeColor = planeMat.color;
            planeColor.a = planeStartAlpha;
            planeMat.color = planeColor;

            // Start internal particles travel
            foreach (DataFlowParticle particle in internalsParticles) {
                particle.Visualize();
            }

            // Stop ground particles travel
            foreach (DataFlowParticle particle in groundParticles) {
                particle.Stop();
            }
        } else if (AvatarHidden) {
            // newAvatarHidden is false, just got out of range
            // Hide text and reset transparency
            foreach (GameObject text in texts) {
                text.SetActive(false);
            }

            ArchetypeManager.Instance.Selected.Model.SetActive(true);
            archetypeMat.SetFloat(AlphaScale, percent);

            // Stop internals particle travel
            foreach (DataFlowParticle particle in internalsParticles) {
                particle.Stop();
            }

            // Begin ground particle travel
            foreach (DataFlowParticle particle in groundParticles) {
                particle.Visualize();
            }
        } else {
            // Adjust transparency
            for (int i = 0; i < boxes.Count; i++) {
                Color boxColor = boxMaterials[i].color;
                boxColor.a = boxAlphas[i] * (1 - percent);
                boxMaterials[i].color = boxColor;

                // Box wireframe color alpha default to 1
                Color wireColor = boxMaterials[i].GetColor(VWireColor);
                wireColor.a = 1 - percent;
                boxMaterials[i].SetColor(VWireColor, wireColor);
            }

            Color planeColor = planeMat.color;
            planeColor.a = planeStartAlpha * (1 - percent);
            planeMat.color = planeColor;
        }

        AvatarHidden = newAvatarHidden;
        
        if (AvatarHidden && !tutorialShown) {
            InternalsTutorial();
            tutorialShown = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        internals.SetActive(false);
        archetypeMat.SetFloat(AlphaScale, 1);
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
    }

    #region Tutorial

    private void InternalsTutorial() {
        TutorialManager.Instance.Pop = true;
        TutorialParam param = new TutorialParam("Tutorials.InternalTitle", "Tutorials.InternalText");
        TutorialManager.Instance.ShowTutorial(param, internalTutorialTransform, () => !AvatarHidden,
            postCallback: () => TutorialManager.Instance.Pop = false, pop: true);
    }

    #endregion
}
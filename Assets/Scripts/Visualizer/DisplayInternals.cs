using UnityEngine;
using UnityEngine.UI;

public class DisplayInternals : MonoBehaviour {
    public float radius = 0.4f;
    public Image image;
    public float attenuation = 0.8f;

    private float startingAlpha;
    private Material XrayMat => PriusManager.Instance.CurrentXRay.GetComponent<Renderer>().sharedMaterial;

    private void Start() {
        GetComponent<SphereCollider>().radius = radius;
        startingAlpha = XrayMat.GetFloat("_AlphaScale");
    }

    private void OnTriggerStay(Collider other) {
        if (other.name.Contains("Camera")) {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            float percent = distance / radius;

            Color imageColor = image.color;
            imageColor.a = (1 - percent) / attenuation;
            image.color = imageColor;

            XrayMat.SetFloat("_AlphaScale", startingAlpha * percent * attenuation);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name.Contains("Camera")) {
            Color imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;

            XrayMat.SetFloat("_AlphaScale", startingAlpha);
        }
    }
}

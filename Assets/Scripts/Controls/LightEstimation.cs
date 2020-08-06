using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraManager))]
public class LightEstimation : MonoBehaviour {
    private ARCameraManager Manager => GetComponent<ARCameraManager>();
    
    [SerializeField] private Light light;
    
    private void OnEnable() {
        Manager.frameReceived += FrameChanged;
    }

    private void OnDisable() {
        Manager.frameReceived -= FrameChanged;
    }
    
    private void FrameChanged(ARCameraFrameEventArgs args) {
        var le = args.lightEstimation;
        if (le.averageBrightness.HasValue) {
            light.intensity = le.averageBrightness.Value;
        }

        if (le.averageColorTemperature.HasValue) {
            light.colorTemperature = le.averageColorTemperature.Value;
        }

        if (le.colorCorrection.HasValue) {
            light.color = le.colorCorrection.Value;
        }

        if (le.mainLightDirection.HasValue) {
            light.transform.rotation = Quaternion.LookRotation(le.mainLightDirection.Value);
        }

        if (le.mainLightColor.HasValue) {
            light.color = le.mainLightColor.Value;
        }

        if (le.mainLightIntensityLumens.HasValue) {
            light.intensity = le.mainLightIntensityLumens.Value;
        }

        if (le.ambientSphericalHarmonics.HasValue) {
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = le.ambientSphericalHarmonics.Value;
        }
    }
}
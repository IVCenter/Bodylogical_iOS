using UnityEngine;

/// <summary>
/// Adjusts several app settings for different devices and platforms.
/// </summary>
public class DeviceManager : MonoBehaviour {
    public static DeviceManager Instance { get; private set; }

    [SerializeField] private DeviceConstants iPhoneConstants;
    [SerializeField] private DeviceConstants iPadConstants;
    [SerializeField] private DeviceConstants editorConstants;

    public DeviceConstants Constants { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        string device = SystemInfo.deviceModel;
        if (Application.isEditor) {
            Constants = editorConstants;
        } else if (device.Contains("iPhone") || Screen.width < 1500) {
            Constants = iPhoneConstants;
        } else if (device.Contains("iPad") || Screen.width >= 1500) {
            Constants = iPadConstants;
        }
    }
}
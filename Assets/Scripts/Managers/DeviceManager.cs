using UnityEngine;

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
        
        if (Application.isEditor) {
            Constants = editorConstants;
        } else {
            string device = SystemInfo.deviceModel;
            if (device.Contains("iPhone")) {
                Constants = iPhoneConstants;
            } else if (device.Contains("iPad")) {
                Constants = iPadConstants;
            }
        }
    }
}
using UnityEngine;

/// <summary>
/// Account for different device sizes.
/// </summary>
[CreateAssetMenu(fileName = "Device Constants", menuName = "Bodylogical/Device Constants", order = 0)]
public class DeviceConstants : ScriptableObject {
    public float startScreenDistance;
    public float tutorialScreenDistance;
    public float stageSize;
}
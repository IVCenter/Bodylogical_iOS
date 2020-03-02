using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A camera script for navigating in the scene with the key and the mouse.
/// This allows testing without having to deploy the app to the iOS device every time.
/// </summary>
public class DebugCamera : MonoBehaviour {
    public float movementSpeed = 0.01f;

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;

    Quaternion originalRotation;

    private void Start() {
        originalRotation = transform.localRotation;
    }

    // Looks for mouse and keyboard movements
    private void Update() {
        if (!InputManager.Instance.menuOpened) {
            Move();
            Look();
        }
    }

    /// <summary>
    /// http://wiki.unity3d.com/index.php/SmoothMouseLook
    /// </summary>
    private void Move() {
        Vector3 movement = new Vector3();

        if (Input.GetKey(KeyCode.W)) {
            movement.z += movementSpeed;
        }

        if (Input.GetKey(KeyCode.S)) {
            movement.z -= movementSpeed;
        }

        if (Input.GetKey(KeyCode.A)) {
            movement.x -= movementSpeed;
        }

        if (Input.GetKey(KeyCode.D)) {
            movement.x += movementSpeed;
        }

        transform.Translate(movement);
    }

    private void Look() {
        rotAverageY = 0f;
        rotAverageX = 0f;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;

        rotArrayY.Add(rotationY);
        rotArrayX.Add(rotationX);

        if (rotArrayY.Count >= frameCounter) {
            rotArrayY.RemoveAt(0);
        }
        if (rotArrayX.Count >= frameCounter) {
            rotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < rotArrayY.Count; j++) {
            rotAverageY += rotArrayY[j];
        }
        for (int i = 0; i < rotArrayX.Count; i++) {
            rotAverageX += rotArrayX[i];
        }

        rotAverageY /= rotArrayY.Count;
        rotAverageX /= rotArrayX.Count;

        rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
        rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

        Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    public static float ClampAngle(float angle, float min, float max) {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F)) {
            if (angle < -360F) {
                angle += 360F;
            }
            if (angle > 360F) {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}

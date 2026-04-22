using UnityEngine;
using UnityEngine.InputSystem;

namespace SABI
{
    public class CameraLook : MonoBehaviour
    {
        public float mouseSensitivity = 500f;
        public Transform playerBody;

        float xRotation = 0f;

        void Start()
        {
            // Lock the cursor to the center of the screen and make it invisible
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // Get mouse input from the new Input System
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
            float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

            // Rotate the player body along the Y axis (horizontal rotation)
            playerBody.Rotate(Vector3.up * mouseX);

            // Rotate the camera along the X axis (vertical rotation) and clamp it so the player can't look too far up or down
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Apply the rotation to the camera's transform
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}

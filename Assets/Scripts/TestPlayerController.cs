using System;
using System.Collections.Generic;
using UnityEngine;

class TestPlayerController : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    public float mouseSensitivity { get; private set; } = 100f;
    public float xRotation { get; private set; } = 0f;

    private CharacterController characterController;
    private float movementSpeed = 6f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movementVector = transform.right * horizontalInput + transform.forward * verticalInput;
        characterController.Move(Vector3.ClampMagnitude(movementVector, 1.0f) * movementSpeed * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up* mouseX);
    }
}

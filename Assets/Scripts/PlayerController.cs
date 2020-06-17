using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.NonSerialized] public float mouseSensitivity = 100f;
    [System.NonSerialized] public bool isGrounded = false;
    [System.NonSerialized] public bool isJumping = false;
    [System.NonSerialized] public bool isWalking = false;
    [System.NonSerialized] public bool isCrouching = false;
    [System.NonSerialized] public float xRotation = 0f;
    [System.NonSerialized] public Vector3 jumpVelocity;

    public Vector3 gunRotation = Vector3.zero;

    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform playerGroundTransform;
    [SerializeField] Animator handAnimator;
    [SerializeField] Transform handsTransform;

    private CharacterController characterController;
    private PlayerStats playerStats;
    private BoxCollider playerCollider;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();
        playerCollider = GetComponent<BoxCollider>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(playerGroundTransform.position, 0.4f, groundLayer);

        HandleJumpInput();
        HandleMovement();
        HandleMouseLook();
    }

    public void SetGunRotation(Vector3 _gunRotation)
    {
        gunRotation = _gunRotation;
        handsTransform.localRotation = Quaternion.Euler(gunRotation);
    }

    void HandleJumpInput()
    {
        bool isTryingToJump = Input.GetKeyDown(KeyCode.Space);

        if (isTryingToJump && isGrounded)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        if (isGrounded && jumpVelocity.y < 0f)
        {
            jumpVelocity.y = -2f;
        }

        if (isJumping)
        {
            jumpVelocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity);
        }

        // Apply gravity
        jumpVelocity.y += playerStats.gravity * Time.deltaTime;

        characterController.Move(jumpVelocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        isWalking = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (z != 0 || x != 0)
        {
            Vector3 movementVector = Vector3.ClampMagnitude((transform.right * x) + (transform.forward * z), 1.0f);
            if (isWalking)
            {
                HandleStand();
                characterController.Move(movementVector * playerStats.walkingMovementSpeed * Time.deltaTime);
            }
            else if (isCrouching)
            {
                HandleCrouch();
                characterController.Move(movementVector * playerStats.crouchingMovementSpeed * Time.deltaTime);
            }
            else
            {
                HandleStand();
                characterController.Move(movementVector * playerStats.runningMovementSpeed * Time.deltaTime);
            }
        } else
        {
            handAnimator.SetBool("Walk", false);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (gunRotation != Vector3.zero)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(
                xRotation + gunRotation.x / 1.2f,
                gunRotation.y / 1.2f,
                gunRotation.z / 1.2f
            );
        } else
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (playerCollider.size.y > playerStats.crouchHeightY)
        {
            Vector3 newSize = new Vector3(playerCollider.size.x, playerStats.crouchHeightY, playerCollider.size.z);
            playerCollider.size = Vector3.Lerp(playerCollider.size, newSize, playerStats.crouchSpeed * Time.deltaTime);
        }
    }

    void HandleStand()
    {
        if (playerCollider.size.y < playerStats.crouchHeightY)
        {
            Vector3 newSize = new Vector3(playerCollider.size.x, playerStats.standingHeightY, playerCollider.size.z);
            playerCollider.size = Vector3.Lerp(playerCollider.size, newSize, playerStats.crouchSpeed * Time.deltaTime);
        }
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.NonSerialized] public float mouseSensitivity = 100f;
    [System.NonSerialized] public bool isMovementDisabled = false;
    [System.NonSerialized] public bool isLookDisabled = false;
    [System.NonSerialized] public bool isGrounded = false;
    [System.NonSerialized] public bool isJumping = false;
    [System.NonSerialized] public bool isWalking = false;
    [System.NonSerialized] public bool isCrouching = false;
    [System.NonSerialized] public float xRotation = 0f;
    [System.NonSerialized] public Vector3 jumpVelocity;
    [System.NonSerialized] public Vector3 inputVector = Vector3.zero;
    [System.NonSerialized] public Vector3 movementVector;

    public Vector3 gunRotation = Vector3.zero;

    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform playerGroundTransform;
    [SerializeField] Animator handAnimator;
    [SerializeField] Transform handsTransform;

    private CharacterController characterController;
    private PlayerStats playerStats;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();

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

    public void SetIsMovementDisabled(bool _isMovementDisabled)
    {
        isMovementDisabled = _isMovementDisabled;
    }

    public void SetIsLookDisabled(bool _isLookDisabled)
    {
        isLookDisabled = _isLookDisabled;
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
        if (isMovementDisabled)
        {
            return;
        }

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.z = Input.GetAxis("Vertical");
        isWalking = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (isCrouching)
        {
            HandleCrouch();
        } else
        {
            HandleStand();
        }

        if (inputVector.z != 0 || inputVector.x != 0)
        {
            movementVector = Vector3.ClampMagnitude((transform.right * inputVector.x) + (transform.forward * inputVector.z), 1.0f);
            if (isWalking)
            {
                characterController.Move(movementVector * playerStats.walkingMovementSpeed * Time.deltaTime);
            }
            else if (isCrouching)
            {
                characterController.Move(movementVector * playerStats.crouchingMovementSpeed * Time.deltaTime);
            }
            else
            {
                characterController.Move(movementVector * playerStats.runningMovementSpeed * Time.deltaTime);
            }
        }
    }

    void HandleMouseLook()
    {
        if (isLookDisabled)
        {
            return;
        }

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
        if (characterController.height > playerStats.crouchHeightY)
        {
            UpdateCharacterHeight(playerStats.crouchHeightY);

            if (characterController.height - 0.05f <= playerStats.crouchHeightY)
            {
                characterController.height = playerStats.crouchHeightY;
            }
        }
    }

    void HandleStand()
    {
        if (characterController.height < playerStats.standingHeightY)
        {
            var lastHeight = characterController.height;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, playerStats.standingHeightY))
            {
                if (hit.distance < playerStats.standingHeightY - playerStats.crouchHeightY)
                {
                    UpdateCharacterHeight(playerStats.crouchHeightY + hit.distance);
                    return;
                } else
                {
                    UpdateCharacterHeight(playerStats.standingHeightY);
                }
            } else
            {
                UpdateCharacterHeight(playerStats.standingHeightY);
            }

            if (characterController.height + 0.05f >= playerStats.standingHeightY)
            {
                characterController.height = playerStats.standingHeightY;
            }

            transform.position += new Vector3(0, (characterController.height - lastHeight) / 2, 0);
        }
    }

    void UpdateCharacterHeight(float newHeight)
    {
        characterController.height = Mathf.Lerp(characterController.height, newHeight, playerStats.crouchSpeed * Time.deltaTime);
    }
}

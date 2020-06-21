using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JettController : MonoBehaviour
{
    public bool isDashing { get; private set; } = false;
    public bool isPullingGunOut { get; private set; } = false;

    [SerializeField] Camera playerCamera = default;
    [SerializeField] ParticleSystem forwardDashParticles = default;
    [SerializeField] ParticleSystem rightDashParticles = default;
    [SerializeField] ParticleSystem backwardDashParticles = default;
    [SerializeField] ParticleSystem leftDashParticles = default;
    [SerializeField] GameObject smokeProjectile = default;
    [SerializeField] Transform smokeFiringTransform = default;

    private int maxDashAttempts = 100;
    private int dashAttempts = 0;
    private float dashStartTime = 0f;

    private int maxSmokeAttempts = 100;
    private int smokeAttempts = 0;

    private PlayerController playerController;
    private PlayerStats playerStats;
    private PlayerWeapon playerWeapon;
    private CharacterController characterController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        playerWeapon = GetComponent<PlayerWeapon>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleDash();

        if (!isDashing)
        {
            HandleSmoke();
        }
    }

    #region Dashing
    void HandleDash()
    {
        bool isTryingDash = Input.GetKeyDown(KeyCode.E);

        if (isTryingDash && !isDashing)
        {
            if (dashAttempts < maxDashAttempts)
            {
                // Start dash
                OnStartDash();
            }
        }

        if (isDashing)
        {
            if (Time.time - dashStartTime <= playerStats.dashDurationSeconds)
            {
                characterController.Move(playerController.movementVector.normalized * playerStats.dashSpeed * Time.deltaTime);
            }
            else
            {
                OnEndDash();
            }
        }
    }

    void OnStartDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        dashAttempts += 1;

        playerWeapon.HideGun();
        PlayDashParticles();

        playerController.SetIsMovementDisabled(true);
        playerController.SetIsLookDisabled(true);
    }

    void OnEndDash()
    {
        isDashing = false;
        dashStartTime = 0f;

        playerWeapon.PullOutGun(() => { });

        playerController.SetIsMovementDisabled(false);
        playerController.SetIsLookDisabled(false);
    }

    void PlayDashParticles()
    {
        Vector3 _inputVector = playerController.inputVector;
        if (_inputVector.z > 0 && Mathf.Abs(_inputVector.x) <= _inputVector.z)
        {
            // Forward & Forward Diagonals
            forwardDashParticles.Play();
            return;
        }
        
        if (_inputVector.z < 0 && Mathf.Abs(_inputVector.x) <= Mathf.Abs(_inputVector.z))
        {
            // Backward & Backward Diagonals
            backwardDashParticles.Play();
            return;
        }
        
        if (_inputVector.x > 0)
        {
            // Right
            rightDashParticles.Play();
            return;
        }
        
        if (_inputVector.x < 0)
        {
            // Left
            leftDashParticles.Play();
        }
}
    #endregion

    void HandleSmoke()
    {
        bool isThrowingSmoke = Input.GetKeyDown(KeyCode.C);

        if (isThrowingSmoke && smokeAttempts < maxSmokeAttempts)
        {
            Debug.Log("isThrowingSmoke");
            Instantiate(smokeProjectile, smokeFiringTransform.position, playerCamera.transform.rotation);

            smokeAttempts += 1;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class JettController : MonoBehaviour
{
    public bool isPullingGunOut { get; private set; } = false;
    public bool isDashing { get; private set; } = false;
    public bool isThrowingSmoke { get; private set; } = false;
    public bool isUpdrafting { get; private set; } = false;
    public bool isFalling { get; private set; } = false;

    [SerializeField] Camera playerCamera = default;
    [SerializeField] ParticleSystem forwardDashParticles = default;
    [SerializeField] ParticleSystem rightDashParticles = default;
    [SerializeField] ParticleSystem backwardDashParticles = default;
    [SerializeField] ParticleSystem leftDashParticles = default;
    [SerializeField] ParticleSystem floatingParticles = default;
    [SerializeField] GameObject smokeProjectile = default;
    [SerializeField] Transform smokeFiringTransform = default;

    private int dashAttempts = 0;
    private float dashStartTime = 0f;

    JettSmokeProjectile currentSmokeProjectile;
    private int smokeAttempts = 0;
    private float lastTimeSmokeEnded = 0f;

    private int updraftAttempts = 0;
    private float lastTimeUpdrafted = 0.0f;

    private float lastJumpVelocityY = 0f;

    private PlayerController playerController;
    private JettStats jettStats;
    private PlayerStats playerStats;
    private PlayerWeapon playerWeapon;
    private CharacterController characterController;
    private JettAnimationController jettAnimationController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        playerWeapon = GetComponent<PlayerWeapon>();
        jettStats = GetComponent<JettStats>();
        jettAnimationController = GetComponent<JettAnimationController>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleDash();
        HandleIsFalling();
        HandleFloat();

        if (!isDashing)
        {
            HandleSmoke();
            HandleUpdraft();
        }
    }

    void HandleIsFalling()
    {
        if (!playerController.isGrounded &&
            playerController.jumpVelocity.y <= 0 &&
            playerController.jumpVelocity.y < lastJumpVelocityY)
        {
            isFalling = true;
        } else
        {
            isFalling = false;
        }

        lastJumpVelocityY = playerController.jumpVelocity.y;
    }

    #region Dashing
    void HandleDash()
    {
        bool isTryingDash = Input.GetKeyDown(KeyCode.E);

        if (isTryingDash && !isDashing)
        {
            if (dashAttempts < jettStats.maxDashAttempts)
            {
                // Start dash
                OnStartDash();
            }
        }

        if (isDashing)
        {
            if (Time.time - dashStartTime <= jettStats.dashDurationSeconds)
            {
                if (playerController.movementVector.Equals(Vector3.zero))
                {
                    // Player is not giving any input so just dash forward
                    characterController.Move(transform.forward * jettStats.dashSpeed * Time.deltaTime);
                } else
                {
                    characterController.Move(playerController.movementVector.normalized * jettStats.dashSpeed * Time.deltaTime);
                }
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
        PlayDash();

        playerController.SetIsMovementDisabled(true);
        playerController.SetIsLookDisabled(true);
    }

    void OnEndDash()
    {
        isDashing = false;
        dashStartTime = 0f;

        playerWeapon.PullOutGun(() => { });
        jettAnimationController.PlayWeaponEquipAnimation();

        playerController.SetIsMovementDisabled(false);
        playerController.SetIsLookDisabled(false);
    }

    void PlayDash()
    {
        Vector3 _inputVector = playerController.inputVector;
        if (_inputVector.z > 0 && Mathf.Abs(_inputVector.x) <= _inputVector.z)
        {
            // Forward & Forward Diagonals
            forwardDashParticles.Play();
            jettAnimationController.PlayForwardDashAnimation();
            return;
        }
        
        if (_inputVector.z < 0 && Mathf.Abs(_inputVector.x) <= Mathf.Abs(_inputVector.z))
        {
            // Backward & Backward Diagonals
            backwardDashParticles.Play();
            jettAnimationController.PlayBackwardDashAnimation();
            return;
        }
        
        if (_inputVector.x > 0)
        {
            // Right
            rightDashParticles.Play();
            jettAnimationController.PlayRightDashAnimation();
            return;
        }
        
        if (_inputVector.x < 0)
        {
            // Left
            leftDashParticles.Play();
            jettAnimationController.PlayLeftDashAnimation();
            return;
        }

        // Default just play forward particles
        forwardDashParticles.Play();
        jettAnimationController.PlayForwardDashAnimation();
}
    #endregion

    #region Smokes
    void HandleSmoke()
    {
        bool isTryingToThrowSmoke = Input.GetKeyDown(KeyCode.C);

        if (isTryingToThrowSmoke && smokeAttempts < jettStats.maxSmokeAttempts)
        {
            if (Time.time - lastTimeSmokeEnded < jettStats.smokeDelaySeconds)
            {
                // We can't machine gun smokes
                Debug.Log(Time.time - lastTimeSmokeEnded);
                return;
            }

            ThrowSmoke();
        }

        if (isThrowingSmoke)
        {
            bool isControlled = Input.GetKey(KeyCode.C);
            currentSmokeProjectile.SetIsControlled(isControlled);

            bool isStoppingControl = Input.GetKeyUp(KeyCode.C);
            if (isStoppingControl)
            {
                OnThrowingSmokeEnd();
            }
        }
    }

    void ThrowSmoke()
    {
        isThrowingSmoke = true;
        playerWeapon.HideGun();
        jettAnimationController.PlayThrowSmokeAnimation();

        GameObject _smokeProjectile = Instantiate(smokeProjectile, smokeFiringTransform.position, playerCamera.transform.rotation);
        currentSmokeProjectile = _smokeProjectile.GetComponent<JettSmokeProjectile>();
        currentSmokeProjectile.InitializeValues(false, playerCamera);

        smokeAttempts += 1;
    }

    void OnThrowingSmokeEnd()
    {
        lastTimeSmokeEnded = Time.time;
        StartCoroutine(PlaySmokeEndTransition());
        isThrowingSmoke = false;
        currentSmokeProjectile.SetIsControlled(false);
        currentSmokeProjectile = null;
    }

    IEnumerator PlaySmokeEndTransition()
    {
        jettAnimationController.PlayThrowSmokeBackToWeapon();
        yield return new WaitForSeconds(jettStats.smokeDelaySeconds);
        playerWeapon.PullOutGun(() => { });
        jettAnimationController.PlayWeaponEquipAnimation();
    }
    #endregion

    #region Updraft
    void HandleUpdraft()
    {
        bool isTryingToUpdraft = Input.GetKeyDown(KeyCode.Q);

        if (Time.time - lastTimeUpdrafted < jettStats.updraftDelaySeconds)
        {
            if (isUpdrafting)
            {
                OnUpdraftEnd();
            }
            return;
        }

        if (isTryingToUpdraft && updraftAttempts < jettStats.maxUpdraftAttempts)
        {
            OnUpdraftStart();
            Updraft();
        }
    }

    void Updraft()
    {
        if (!playerController.isGrounded)
        {
            playerController.jumpVelocity.y = Mathf.Sqrt((jettStats.updraftHeight / 2.5f) * -2f * playerStats.gravity);
        }
        else
        {
            playerController.jumpVelocity.y = Mathf.Sqrt(jettStats.updraftHeight * -2f * playerStats.gravity);
        }
    }

    void OnUpdraftStart()
    {
        isUpdrafting = true;
        lastTimeUpdrafted = Time.time;
        playerWeapon.HideGun();
    }

    void OnUpdraftEnd()
    {
        isUpdrafting = false;
        playerWeapon.PullOutGun(() => { });
    }
    #endregion

    #region Floating
    void HandleFloat()
    {
        //Debug.Log("HandleFloat");
        bool isTryingToFloat = isFalling && Input.GetKey(KeyCode.Space);

        if (isTryingToFloat)
        {
            if (floatingParticles.isStopped)
            {
                floatingParticles.Play();
            }
            playerStats.gravity = JettStats.FloatingGravity;
        } else
        {
            if (floatingParticles.isPlaying)
            {
                floatingParticles.Stop();
            }
            playerStats.gravity = PlayerStats.DefaultGravity;
        }
    }
    #endregion
}

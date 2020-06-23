using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JettAnimationController : MonoBehaviour
{
    [SerializeField] Animator armsAnimator;

    private const float defaultAnimationSpeed = 2;

    void Start()
    {
        armsAnimator.speed = defaultAnimationSpeed;
    }

    public void PlayForwardDashAnimation()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("ForwardDash");
    }

    public void PlayRightDashAnimation()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("RightDash");
    }

    public void PlayBackwardDashAnimation()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("BackwardDash");
    }

    public void PlayLeftDashAnimation()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("LeftDash");
    }

    public void PlayWeaponEquipAnimation()
    {
        armsAnimator.speed = defaultAnimationSpeed;
        armsAnimator.Play("WeaponEquip");
    }

    public void PlayThrowSmokeAnimation()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("ThrowSmoke");
    }

    public void PlayThrowSmokeBackToWeapon()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("ThrowSmokeBackToWeapon");
    }
}

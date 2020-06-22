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

    public void PlayDashAnimation()
    {
        armsAnimator.speed = 4;
        armsAnimator.Play("ForwardDash");
    }

    public void PlayWeaponEquipAnimation()
    {
        armsAnimator.speed = defaultAnimationSpeed;
        armsAnimator.Play("WeaponEquip");
    }
}

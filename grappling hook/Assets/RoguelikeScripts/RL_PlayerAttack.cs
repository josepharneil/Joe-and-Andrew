using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private SpriteRenderer weaponRender;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float attackSpeed = 2;

    [Header("Debug")]
    [Range(0, 360)]
    [SerializeField] private float weaponInitialRotationZ = 70;
    [Range(0, 360)]
    [SerializeField] private float weaponFinalRotationZ = 20;

    // Used for drawing lines in editor.
    private float debugPreviousWeaponInitialRotationZ = 70;
    private float debugPreviousWeaponFinalRotationZ = 70;


    private enum AttackState
    {
        NotAttacking,
        Start,
        Attacking,
        End
    }
    private AttackState attackState = AttackState.NotAttacking;

    private void Awake()
    {
        weaponRender.enabled = false;
        debugPreviousWeaponInitialRotationZ = weaponInitialRotationZ;
        debugPreviousWeaponFinalRotationZ = weaponFinalRotationZ;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleAttackInput();
        // No rigidbody, so put in update.
        ApplyAttack();

        // Debug:
        DebugDrawAttackLines();
    }

    private void DebugDrawAttackLines()
    {
        if (weaponInitialRotationZ != debugPreviousWeaponInitialRotationZ)
        {
            const float length = 3f;
            debugPreviousWeaponInitialRotationZ = weaponInitialRotationZ;
            Debug.DrawRay(weaponTransform.position, length *
                new Vector3(Mathf.Cos(weaponInitialRotationZ * (Mathf.PI / 180f)), Mathf.Sin(weaponInitialRotationZ * (Mathf.PI / 180f)), 0),
                Color.white, 0.05f);
        }
        if (weaponFinalRotationZ != debugPreviousWeaponFinalRotationZ)
        {
            const float length = 3f;
            debugPreviousWeaponFinalRotationZ = weaponFinalRotationZ;
            Debug.DrawRay(weaponTransform.position, length *
                new Vector3(Mathf.Cos(weaponFinalRotationZ * (Mathf.PI / 180f)), Mathf.Sin(weaponFinalRotationZ * (Mathf.PI / 180f)), 0),
                Color.red, 0.05f);
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && attackState == AttackState.NotAttacking)
        {
            attackState = AttackState.Start;
        }
    }

    private void ApplyAttack()
    {
        if (attackState == AttackState.Start)
        {
            ApplyStartAttack();   
        }
        if (attackState == AttackState.Attacking)
        {
            ApplyUpdateAttack();
        }
        if (attackState == AttackState.End)
        {
            ApplyEndAttack();
        }
    }

    private void ApplyStartAttack()
    {
        // TODO maybe set rotation based on facing direction here?
        weaponTransform.rotation = Quaternion.Euler(weaponTransform.rotation.eulerAngles.x,
        weaponTransform.rotation.eulerAngles.y, weaponInitialRotationZ);

        weaponRender.enabled = true;
        attackState = AttackState.Attacking;
    }

    private void ApplyUpdateAttack()
    {
        // @JA TODO Fix this, attack doesnt seem to go whole distance.
        // its quite inconsistent where it gets to.
        // Maybe rotate based on attackSpeed
        // Like lerp from initial -> final in fewer steps if higher attack speed
        // and use more steps if lower attack speeds.
        // This is my next todo (feel free to try it yourself)
        if( weaponInitialRotationZ < weaponFinalRotationZ )
        {
            weaponTransform.Rotate(new Vector3(0, 0, 1), attackSpeed);
            if (weaponTransform.eulerAngles.z >= weaponFinalRotationZ)
            {
                attackState = AttackState.End;
            }
        }
        else
        {
            weaponTransform.Rotate(new Vector3(0, 0, 1), -attackSpeed);
            if (weaponTransform.eulerAngles.z <= weaponFinalRotationZ)
            {
                attackState = AttackState.End;
            }
        }
    }

    private void ApplyEndAttack()
    {
        weaponRender.enabled = false;
        attackState = AttackState.NotAttacking;
        weaponTransform.rotation = Quaternion.Euler(weaponTransform.rotation.eulerAngles.x,
            weaponTransform.rotation.eulerAngles.y, weaponInitialRotationZ);
    }
}

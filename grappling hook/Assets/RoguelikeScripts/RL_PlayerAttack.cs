using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer weaponRender;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private RL_PlayerStats playerStats;

    // TODO bad way of doing this, shouldn't really have a reference to the controller.
    [SerializeField] private RL_PlayerController rlPlayerController;
    [Header("Config")]
    [SerializeField] private float attackSpeed = 2;
    [Range(0, 360)]
    [SerializeField] private float weaponInitialRotationZ = 70;
    [Range(0, 360)]
    [SerializeField] private float weaponFinalRotationZ = 295;

    [Header("Debug")]
    [Range(0, 360)]
    [SerializeField] private float debugCurrentRotationZ = 0;

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
    private RL_PlayerController.FacingDirection facingDirectionAtAttackTime;

    public bool IsNotAttacking()
    {
        return attackState == AttackState.NotAttacking;
    }

    private void Awake()
    {
        weaponRender.enabled = false;
        debugPreviousWeaponInitialRotationZ = weaponInitialRotationZ;
        debugPreviousWeaponFinalRotationZ = weaponFinalRotationZ;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!playerStats.IsPlayerDead())
        {
            HandleAttackInput();
            // No rigidbody, so put in update.
            ApplyAttack();

            // Debug:
            DebugDrawAttackLines();
        }
    }

    private void DebugDrawAttackLines()
    {
        if (weaponInitialRotationZ != debugPreviousWeaponInitialRotationZ)
        {
            const float length = 3f;
            debugPreviousWeaponInitialRotationZ = weaponInitialRotationZ;
            Debug.DrawRay(weaponTransform.position, length *
                new Vector3(-Mathf.Sin(weaponInitialRotationZ * (Mathf.PI / 180f)), Mathf.Cos(weaponInitialRotationZ * (Mathf.PI / 180f)), 0),
                Color.white, 0.05f);
        }
        if (weaponFinalRotationZ != debugPreviousWeaponFinalRotationZ)
        {
            const float length = 3f;
            debugPreviousWeaponFinalRotationZ = weaponFinalRotationZ;
            Debug.DrawRay(weaponTransform.position, length *
                new Vector3(-Mathf.Sin(weaponFinalRotationZ * (Mathf.PI / 180f)), Mathf.Cos(weaponFinalRotationZ * (Mathf.PI / 180f)), 0),
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
        facingDirectionAtAttackTime = rlPlayerController.GetFacingDirection();
        if (facingDirectionAtAttackTime == RL_PlayerController.FacingDirection.Right)
        {
            weaponTransform.rotation = Quaternion.Euler(weaponTransform.rotation.eulerAngles.x,
                weaponTransform.rotation.eulerAngles.y, weaponInitialRotationZ);
        }
        else
        {
            float leftWeaponInitialRotationZ = (360 - weaponInitialRotationZ);
            weaponTransform.rotation = Quaternion.Euler(weaponTransform.rotation.eulerAngles.x,
                weaponTransform.rotation.eulerAngles.y, leftWeaponInitialRotationZ);
        }

        weaponRender.enabled = true;
        attackState = AttackState.Attacking;
    }

    private void ApplyUpdateAttack()
    {
        Debug.Assert(weaponInitialRotationZ != weaponFinalRotationZ);
        if(facingDirectionAtAttackTime == RL_PlayerController.FacingDirection.Right)
        {
            ApplyUpdateAttackFacingRight();
        }
        else
        {
            ApplyUpdateAttackFacingLeft();
        }
        debugCurrentRotationZ = weaponTransform.eulerAngles.z;
    }

    private void ApplyUpdateAttackFacingRight()
    {
        // Rotate clockwise
        if (weaponInitialRotationZ > weaponFinalRotationZ)
        {
            // @JA TODO Ingore this for now, need to think more about how we
            // actually want to attacks
            //float speedToUse = attackSpeed;
            //float currRotZ = weaponTransform.eulerAngles.z;
            //float midPoint = (weaponInitialRotationZ + weaponFinalRotationZ) / 2f;
            //float distToMidPoint = Mathf.Abs(currRotZ - midPoint);
            //// The further we are from the mid point, the slower we go.
            //float diffSq = distToMidPoint * distToMidPoint;
            //if(diffSq == 0)
            //{
            //    diffSq = 0.01f;
            //}
            //speedToUse /= (0.25f*diffSq);


            weaponTransform.Rotate(new Vector3(0, 0, 1), -attackSpeed);
            // We have to go through 360, so we get a wrap-around, so we need to check between.
            float newWeaponRotationZ = weaponTransform.eulerAngles.z;
            if ( newWeaponRotationZ < weaponFinalRotationZ)
            {
                attackState = AttackState.End;
            }
        }
        //// Anti-clockwise
        //else
        //{
        //    weaponTransform.Rotate(new Vector3(0, 0, 1), attackSpeed);
        //    float newWeaponRotationZ = weaponTransform.eulerAngles.z;
        //    if (weaponFinalRotationZ < newWeaponRotationZ && newWeaponRotationZ < weaponInitialRotationZ)
        //    {
        //        attackState = AttackState.End;
        //    }
        //}
    }

    private void ApplyUpdateAttackFacingLeft()
    {
        float leftWeaponFinalRotationZ = (360 - weaponFinalRotationZ);
        // Anticlockwise
        if (weaponInitialRotationZ > weaponFinalRotationZ)
        {
            weaponTransform.Rotate(new Vector3(0, 0, 1), attackSpeed);
            float newWeaponRotationZ = weaponTransform.eulerAngles.z;

            if (newWeaponRotationZ > leftWeaponFinalRotationZ)
            {
                attackState = AttackState.End;
            }
        }
        //// Clockwise
        //else
        //{
        //    weaponTransform.Rotate(new Vector3(0, 0, 1), attackSpeed);
        //    float newWeaponRotationZ = weaponTransform.eulerAngles.z;
        //    if (leftWeaponFinalRotationZ < newWeaponRotationZ && newWeaponRotationZ < leftWeaponInitialRotationZ)
        //    {
        //        attackState = AttackState.End;
        //    }
        //}
    }

    private void ApplyEndAttack()
    {
        weaponRender.enabled = false;
        attackState = AttackState.NotAttacking;
        weaponTransform.rotation = Quaternion.Euler(weaponTransform.rotation.eulerAngles.x,
            weaponTransform.rotation.eulerAngles.y, weaponInitialRotationZ);
    }
}

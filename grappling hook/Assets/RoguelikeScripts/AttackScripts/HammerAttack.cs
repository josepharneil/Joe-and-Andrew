using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HammerAttack : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private SpriteRenderer weaponRender;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private RL_PlayerController playerController;
    [SerializeField] private BoxCollider2D weaponCollider;

    private Quaternion initialParentRotation;
    private Vector3 initialPosition;

    private enum AttackState
    {
        NotAttacking,
        Start,
        Attacking,
        End
    }

    [Header("Debug")]
    [SerializeField] private AttackState attackState;
    private RL_PlayerController.FacingDirection facingDirection;

    private void Awake()
    {
        attackState = AttackState.NotAttacking;
        weaponRender.enabled = false;
        weaponCollider.enabled = false;
        initialParentRotation = weaponTransform.rotation;
        initialPosition = weaponTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        RunAttack();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            attackState = AttackState.Start;
        }
    }

    void RunAttack()
    {
        switch (attackState)
        {
            case AttackState.NotAttacking:
                break;
            case AttackState.Start:
                StartAttack();
                break;
            case AttackState.Attacking:
                break;
            case AttackState.End:
                EndAttack();
                break;

        }
    }
    void StartAttack()
    {
        facingDirection = playerController.GetFacingDirection();
        weaponRender.enabled = true;
        weaponCollider.enabled = true;
        //weaponTransform.rotation = initialParentRotation;
        
        attackState = AttackState.Attacking;
        DoAttack();
    }

    void DoAttack()
    {
        weaponTransform.DORotate(new Vector3(0,0,-135f * (float)facingDirection),0.5f).OnComplete(() =>
        {
            attackState = AttackState.End;
        });
        
    }

   void EndAttack()
    {
        weaponTransform.rotation = initialParentRotation;
        attackState = AttackState.NotAttacking;
        weaponRender.enabled = false;
        weaponCollider.enabled = false;
        
        
    }
}

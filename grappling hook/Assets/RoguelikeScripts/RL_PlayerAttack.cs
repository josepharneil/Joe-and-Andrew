using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerAttack : MonoBehaviour
{
    private bool isAttackStart;
    private bool isAttacking;
    private bool isAttackEnd;
    private float attackDuration;
    private bool isAttackingThisUpdate;
    private float attackTimer = float.MaxValue;
    //private Raycast hit;
   public GameObject weapon;
  
    
    void Awake()
    {
        weapon.GetComponent<SpriteRenderer>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        HandleAttackInput();
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttackStart = true;
        }
    }
    void FixedUpdate()
    {
        ApplyAttack();
    }

    private void ApplyAttack()
    {
        UpdateAttackState();
        if (isAttackStart)
        {
            ApplyStartAttack();
            
        }
        /*UpdateAttackState();
        if (isAttacking)
        {
            ApplyUpdateAttack();
        }

        if (isAttackEnd)
        {
            ApplyEndAttack();
        }*/
    }

    private void UpdateAttackState()
    {
        isAttackingThisUpdate = attackTimer < attackDuration;
        if (isAttacking && !isAttackingThisUpdate)
        {
            isAttackEnd = true;
        }
        isAttacking = isAttackingThisUpdate;
    }

    private void ApplyStartAttack()
    {
        weapon.GetComponent<SpriteRenderer>().enabled = true;
    }
}

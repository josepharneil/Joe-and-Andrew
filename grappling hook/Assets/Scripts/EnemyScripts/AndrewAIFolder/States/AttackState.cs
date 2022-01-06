using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float attackRange;

    public StateTransition startAttacking;
    private void Awake()
    {
        startAttacking = new StateTransition(this, isInRange());
        transitions.Add(startAttacking, 0);
    }
    public override Type Tick()
    {
        Debug.Log("I am attacking the player");
        return null;
    }

    Func<bool> isInRange() =>()=> Vector2.Distance(playerTransform.position, gameObject.transform.position) <= attackRange;

}

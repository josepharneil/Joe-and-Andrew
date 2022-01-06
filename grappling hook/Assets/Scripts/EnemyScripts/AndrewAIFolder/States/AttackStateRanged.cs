using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateRanged : BaseState
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minAttackRange;
    [SerializeField] private float maxAttackRange;

    StateTransition startAttacking;

    private void Awake()
    {
        startAttacking = new StateTransition(this, isInRange());
        transitions.Add(startAttacking, 0);
    }
    public override Type Tick()
    {
        Debug.Log("I am doing a ranged attack");
        return null;
    }


    Func<bool> isInRange() => () => Vector2.Distance(playerTransform.position, gameObject.transform.position) < maxAttackRange
        && Vector2.Distance(playerTransform.position, gameObject.transform.position) > minAttackRange;
}

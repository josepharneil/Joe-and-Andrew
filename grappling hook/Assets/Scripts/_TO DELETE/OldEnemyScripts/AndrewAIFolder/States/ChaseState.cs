using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float chaseRange;
    public StateTransition startChasing;
    public StateTransition stopChasing;
    public void Awake()
    {   
        startChasing = new StateTransition(this, isInRange());
        //stopChasing = new StateTransition(this, isOutOfRange());
        transitions.Add(startChasing, 1);
        //transitions.Add(stopChasing, 0);
    }

    public override Type Tick()
    {
        Debug.Log("I am chasing the player");
        return null;
    }

    Func<bool> isInRange() => () => Vector2.Distance(playerTransform.position, gameObject.transform.position) <= chaseRange;
    Func<bool> isOutOfRange() => () => Vector2.Distance(playerTransform.position, gameObject.transform.position) > chaseRange;

}

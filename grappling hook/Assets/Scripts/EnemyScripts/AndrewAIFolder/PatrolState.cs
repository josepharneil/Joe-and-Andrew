using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float sightDistance;
    [SerializeField] private float patrolDistance;
    [SerializeField] private float moveSpeed;

    private Vector2 initialPosition;
    private Vector2 goalPosition;
    private Vector2 destinationPosition;

    public void OnEnter(Vector2 initialPosition)
    {
        this.initialPosition = initialPosition;
        this.goalPosition=new Vector2(initialPosition.x + patrolDistance,initialPosition.y);
    }

    public override Type Tick()
    {
        if (Mathf.Abs(Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(playerTransform.position.x)) <= sightDistance)
        {
            return typeof(ChaseState);
        }
        DoPathing();
        Debug.Log("I am patrolling");
        return null;
    }
    
    void DoPathing()
    {
        SetDestinationPosition();
        gameObject.transform.Translate(Mathf.Sign(destinationPosition.x - gameObject.transform.position.x)*moveSpeed*Time.deltaTime,0f,0f);
    }

    void SetDestinationPosition()
    {
        if (Mathf.Approximately(0f, Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(initialPosition.x)))
        {
            destinationPosition = goalPosition;
        }
        if (Mathf.Approximately(0f, Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(goalPosition.x)))
        {
            destinationPosition = initialPosition;
        }
    }

}

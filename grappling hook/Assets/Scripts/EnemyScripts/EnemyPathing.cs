using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float accelerationRate;
    [SerializeField] private EnemyMovement enemyMovement;

    private Vector3 startingPosition;
    private Vector3 goalPosition;
    private float patrolDistance= 2f;
    private float patrolDirection = -1f;
    private float currentPatrolDirection;
    private EnemyMovement movement;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    private float currentSpeed = 0f;
    private float lerpCurrent = 0f;

    private void Start()
    {
        startingPosition = gameObject.GetComponent<Transform>().position;
        goalPosition = new Vector3(startingPosition.x + patrolDistance * patrolDirection, startingPosition.y, 0f);
    }

    public void UpdateReturnToPatrol()
    {
        // This just makes enemy return to the starting position.
        WalkTowards(startingPosition);
        // nearly equal, just teleport
        if( Mathf.Abs(transform.position.x - startingPosition.x) < 0.2f)
        {
            transform.position = startingPosition;
            currentPatrolDirection = patrolDirection;
        }
    }

    public void UpdatePatrol()
    {
        if (Mathf.Abs(Mathf.Abs(transform.position.x) -Mathf.Abs(startingPosition.x)) < 0.2f)
        {
            currentPatrolDirection = patrolDirection;
        }
        if(Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(goalPosition.x) )< 0.2f)
        {
            currentPatrolDirection = patrolDirection * -1f;
        }
        if (currentPatrolDirection == patrolDirection)
        {
            WalkTowards(goalPosition);
        }
        else
        {
            WalkTowards(startingPosition);
        }
        
    }

    public void UpdateChasePlayer()
    {
        RunTowards(playerTransform.position);
    }

    private void WalkTowards(Vector3 target)
    {
        GoTowardsAtSpeed(target, walkSpeed);   
    }

    private void RunTowards(Vector3 target)
    {

        GoTowardsAtSpeed(target,runSpeed);

    }


    private void GoTowardsAtSpeed( Vector3 target, float speed )
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        if (moveDirection * currentSpeed != speed)
        {
            Accelerate(target, speed);
        }
        else
        {
            lerpCurrent = 0f;
        }
        enemyMovement.Move(new Vector2(currentSpeed * Time.deltaTime, 0f));
    }

    private void Accelerate(Vector3 target,float goalSpeed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        currentSpeed = Mathf.Lerp(currentSpeed, goalSpeed * moveDirection, accelerationCurve.Evaluate(lerpCurrent));
    }
}

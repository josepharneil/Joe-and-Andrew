using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBackAndForth : PatrolPathing
{
    private Vector2 goalPosition;
    [SerializeField] private float patrolDistanceX;
    [SerializeField] private float patrolDistanceY;
    [SerializeField] private float moveSpeed;
    private float patrolDirection;
    private float currentPatrolDirection;

    public override void SetUpWayPoints()
    {
        base.SetUpWayPoints();
        goalPosition = new Vector2(startingPosition.x+patrolDistanceX,startingPosition.y+patrolDistanceY);
        patrolDirection = Mathf.Sign(patrolDistanceX);
    }
    public override void UpdatePatrol()
    {
        //ToDo AK: update this so that it can be used properly for y translating enemies
        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(startingPosition.x)) < 0.2f)
        {
            currentPatrolDirection = patrolDirection;
        }
        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(goalPosition.x)) < 0.2f)
        {
            currentPatrolDirection = patrolDirection * -1f;
        }
        if (currentPatrolDirection == patrolDirection)
        {
            GoTowardsAtSpeed(goalPosition,moveSpeed);
        }
        else
        {
            GoTowardsAtSpeed(startingPosition,moveSpeed);
        }
    }
}

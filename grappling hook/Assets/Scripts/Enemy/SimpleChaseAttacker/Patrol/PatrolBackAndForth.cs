using UnityEngine;
using Entity;

public class PatrolBackAndForth : PatrolPathing
{
    [SerializeField] private float patrolDistanceX;
    [SerializeField] private float patrolDistanceY;
    [SerializeField] private float moveSpeed;
    private Vector2 _goalPosition;
    private FacingDirection _patrolDirection;
    private FacingDirection _currentPatrolDirection = FacingDirection.Right;

    protected override void SetUpWayPoints()
    {
        base.SetUpWayPoints();
        _goalPosition = new Vector2(StartingPosition.x+patrolDistanceX,StartingPosition.y+patrolDistanceY);
        _patrolDirection = (FacingDirection)(int)Mathf.Sign(patrolDistanceX);
    }
    
    public override void UpdatePatrol()
    {
        //ToDo AK: update this so that it can be used properly for y translating enemies
        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(StartingPosition.x)) < 0.2f)
        {
            _currentPatrolDirection = _patrolDirection;
        }
        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(_goalPosition.x)) < 0.2f)
        {
            _currentPatrolDirection = (FacingDirection)((int)_patrolDirection * -1);
        }

        GoTowardsAtSpeed(_currentPatrolDirection == _patrolDirection ? _goalPosition : StartingPosition, moveSpeed);
    }
}

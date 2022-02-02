using System.Collections.Generic;
using Entity;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public abstract class PatrolPathing : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MovementController movement;

    [Header("Stats")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float accelerationRate;
    
    protected Vector2 StartingPosition;
    private List<Vector2> _wayPoints;
    private float _lerpCurrent = 0f;
    private float _currentSpeed = 0f;

    public void Awake()
    {
        SetUpWayPoints();
    }

    protected virtual void SetUpWayPoints()
    {
        //gets the initial position and sets it as the first waypoint
        StartingPosition = gameObject.transform.position;
        _wayPoints = new List<Vector2> { StartingPosition };
    }
    
    //ToDo AK: Make this work for y movement
    protected void GoTowardsAtSpeed(Vector2 target, float speed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        // todo
        // if (Math.Abs(moveDirection * _currentSpeed - speed) > TOLERANCE)
        if (moveDirection * _currentSpeed != speed)
        {
            Accelerate(target, speed);
        }
        else
        {
            _lerpCurrent = 0f;
        }
        movement.Move(new Vector2(_currentSpeed, 0f));
    }

    private void Accelerate(Vector2 target, float goalSpeed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        _currentSpeed = Mathf.Lerp(_currentSpeed, goalSpeed * moveDirection, accelerationCurve.Evaluate(_lerpCurrent));
    }

    //this is the method that is called from the PatrolAction scriptable object
    public abstract void UpdatePatrol();
}

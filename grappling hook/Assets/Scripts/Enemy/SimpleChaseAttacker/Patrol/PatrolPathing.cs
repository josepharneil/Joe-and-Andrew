using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EnemyMovement))]
public abstract class PatrolPathing : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyMovement movement;

    [Header("Stats")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float accelerationRate;


    protected Vector2 startingPosition;
    protected List<Vector2> wayPoints;
    private float lerpCurrent = 0f;
    private float currentSpeed = 0f;

    public void Awake()
    {
        SetUpWayPoints();
    }

    public virtual void SetUpWayPoints()
    {
        //gets the initial position and sets it as the first waypoint
        startingPosition = gameObject.transform.position;
        wayPoints = new List<Vector2> { startingPosition };
    }
    //ToDo AK: Make this work for y movement
    public virtual void GoTowardsAtSpeed(Vector2 target, float speed)
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
        movement.Move(new Vector2(currentSpeed * Time.deltaTime, 0f));
    }

    public virtual void Accelerate(Vector2 target, float goalSpeed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        currentSpeed = Mathf.Lerp(currentSpeed, goalSpeed * moveDirection, accelerationCurve.Evaluate(lerpCurrent));
    }

    //this is the method that is called from the PatrolAction scriptable object
    public abstract void UpdatePatrol();
}

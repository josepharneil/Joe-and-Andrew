using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EnemyMovement))]
public abstract class ChasePathing : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected EnemyMovement movement;
    [SerializeField] protected Transform playerTransform;

    [Header("Stats")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float accelerationRate;
    [SerializeField] protected float moveSpeed;
    [SerializeField] public float sightRange;
    [SerializeField] public float sightWidth;

    private float lerpCurrent = 0f;
    private float currentSpeed = 0f;

    //Todo AK: combine gotowards and accelerate into a pathing class
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
        movement.Move(new Vector2(currentSpeed * Time.deltaTime, currentSpeed * Time.deltaTime));
    }

    public virtual void Accelerate(Vector2 target, float goalSpeed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        currentSpeed = Mathf.Lerp(currentSpeed, goalSpeed * moveDirection, accelerationCurve.Evaluate(lerpCurrent));
    }

    //this is the method that is called from the PatrolAction scriptable object
    public abstract void UpdateChase();
}

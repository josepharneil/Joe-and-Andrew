using Entity;
using UnityEngine;

/// <summary>
/// Base class for use in the ChaseState.
/// </summary>
[RequireComponent(typeof(MovementController))]
public abstract class ChasePathing : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected MovementController movement;
    [SerializeField] protected Transform playerTransform;

    [Header("Stats")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float accelerationRate;
    [SerializeField] protected float moveSpeed;
    [SerializeField] public float sightRange;
    [SerializeField] public float sightWidth;
    [SerializeField] public int sightHeight;
    [SerializeField] public LayerMask mask;

    private float _lerpCurrent = 0f;
    private float _currentSpeed = 0f;

    //Todo AK: combine gotowards and accelerate into a pathing class
    protected void GoTowardsAtSpeed(Vector2 target, float speed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        // TODO: Tolerance, not equivalence.
        //if (Math.Abs(moveDirection * _currentSpeed - speed) > TOLERANCE)
        if (moveDirection * _currentSpeed != speed)
        {
            Accelerate(target, speed);
        }
        else
        {
            _lerpCurrent = 0f;
        }
        //ToDo AK: allow for decent vertical movement
        movement.MoveAtSpeed(new Vector2(_currentSpeed, 0f));
    }

    private void Accelerate(Vector2 target, float goalSpeed)
    {
        float moveDirection = Mathf.Sign(target.x - transform.position.x);
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        _currentSpeed = Mathf.Lerp(_currentSpeed, goalSpeed * moveDirection, accelerationCurve.Evaluate(_lerpCurrent));
    }

    // This is the method that is called from the PatrolAction scriptable object
    public abstract void UpdateChase();
}

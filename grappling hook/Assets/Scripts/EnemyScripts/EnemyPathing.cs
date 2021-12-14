using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    private float currentSpeed = 0f;

    public void UpdateReturnToPatrol()
    {
        // This just makes enemy return to position 10.
        WalkTowards(new Vector3(15, 0, 0));
        // nearly equal, just teleport
        if( Mathf.Abs(transform.position.x - 15f) < 0.2f )
        {
            transform.position = new Vector3(15f, transform.position.y, transform.position.z);
        }
    }

    public void UpdatePatrol()
    {
        // This enemy has no patrol... just stands there for now...
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
        GoTowardsAtSpeed(target, runSpeed);
    }


    private void GoTowardsAtSpeed( Vector3 target, float speed )
    {
        // Assumes flat surface

        // If this transform's position is less than the targets, we need to go the right
        if (transform.position.x < target.x)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, speed, 0.2f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, -speed, 0.2f);
        }
        transform.position += new Vector3(1, 0, 0) * Time.deltaTime * currentSpeed;
    }
}

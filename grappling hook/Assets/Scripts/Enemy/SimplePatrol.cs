using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace AI
{
    
    public class SimplePatrol : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController movement;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private float distanceThreshold = 1f;

        [SerializeField] private bool ignorePatrolPointY = true;
        [SerializeField] private bool isFlyingPath = false;
        [SerializeField] private bool movesFasterFurtherAway = false;
        
        [SerializeField] private PatrolPath patrolPath;
        
        private FacingDirection _facingDirection = FacingDirection.Right;
        private const float Speed = 2f;

        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        private void OnValidate()
        {
            patrolPath.Validate();
        }

        public void UpdatePatrol()
        {
            MoveTowardsDestination();
        }

        private void MoveTowardsDestination()
        {
            Transform targetPatrolPoint = UpdateTargetPatrolPoint();

            if (isFlyingPath)
            {
                FlyTowardsTarget(targetPatrolPoint);
            }
            else
            {
                WalkTowardsTarget(targetPatrolPoint);
            }
            _spriteRenderer.flipX = _facingDirection != FacingDirection.Right;
        }

        private void FlyTowardsTarget(Transform targetPatrolPoint)
        {
            Vector2 targetPosition = targetPatrolPoint.position;
            Vector2 thisPosition = transform.position;
            Vector2 direction = (targetPosition - thisPosition);
            Vector2 moveVector = direction * Speed;
            if (!movesFasterFurtherAway)
            {
                moveVector.Normalize();
            }

            movement.Move(moveVector);
            if (moveVector.x > 0)
            {
                _facingDirection = FacingDirection.Left;
            }
            else if (moveVector.x > 0)
            {
                _facingDirection = FacingDirection.Right;
            }
        }

        private Transform UpdateTargetPatrolPoint()
        {
            Transform targetPatrolPoint = patrolPath.GetCurrentPatrolPoint();

            bool isAtPatrolPoint = false;
            if (ignorePatrolPointY)
            {
                // If we're close enough to our destination point, update the destination
                float targetPatrolPointX = targetPatrolPoint.position.x;
                float thisPositionX = transform.position.x;
                isAtPatrolPoint = Mathf.Abs(thisPositionX - targetPatrolPointX) < distanceThreshold;
            }
            else
            {
                Vector2 targetPatrolPointV2 = targetPatrolPoint.position;
                Vector2 thisPosition = transform.position;
                float num1 = targetPatrolPointV2.x - thisPosition.x;
                float num2 = targetPatrolPointV2.y - thisPosition.y;
                float sqDistance = (float)((double) num1 * (double) num1 + (double) num2 * (double) num2);
                isAtPatrolPoint = sqDistance < distanceThreshold * distanceThreshold;
            }
            
            if (isAtPatrolPoint)
            {
                targetPatrolPoint = patrolPath.SetNextPatrolPoint();
            }

            return targetPatrolPoint;
        }
        
        private void WalkTowardsTarget(Transform currentTargetPatrolPoint)
        {
            // Update facing direction
            if (transform.position.IsRightOf(currentTargetPatrolPoint.position))
            {
                _facingDirection = FacingDirection.Left;
            }
            else if (transform.position.IsLeftOf(currentTargetPatrolPoint.position))
            {
                _facingDirection = FacingDirection.Right;
            }
            float fallSpeed = !movement.customCollider2D.GetCollisionBelow() ? Physics2D.gravity.y : 0f;
            Vector2 moveVector = new Vector2((float)_facingDirection * Speed, fallSpeed);
            movement.Move(moveVector);
        }

        private void OnDrawGizmosSelected()
        {
            const float wireSphereRadius = 0.5f;
            
            List<Transform> patrolPoints = patrolPath.GetPatrolPoints();
            if (patrolPoints.Count == 0) return;
            
            for (var index = 0; index < patrolPoints.Count - 1; index++)
            {
                // Draw current patrol point sphere
                Transform currPatrolPoint = patrolPoints[index];
                if (currPatrolPoint == null) continue;
                Vector3 currPatrolPointPosition = currPatrolPoint.position;
                Gizmos.DrawWireSphere(currPatrolPointPosition, wireSphereRadius);

                // Draw line to next point
                Transform nextPatrolPoint = patrolPoints[index + 1];
                if (nextPatrolPoint == null) continue;
                Vector3 nextPatrolPointPosition = nextPatrolPoint.position;

                Gizmos.DrawLine(currPatrolPointPosition, nextPatrolPointPosition);
            }
            
            // Draw last sphere
            Transform lastPatrolPoint = patrolPoints[patrolPoints.Count - 1];
            if (lastPatrolPoint != null)
            {
                Gizmos.DrawWireSphere(patrolPoints[patrolPoints.Count - 1].position, wireSphereRadius);

                // If its a cycle, draw a line from the end to the start
                if (patrolPath.GetPatrolType() == PatrolPath.PatrolType.Cycle)
                {
                    Transform firstPatrolPoint = patrolPoints[0];
                    if (firstPatrolPoint != null)
                    {
                        Gizmos.DrawLine(lastPatrolPoint.position, firstPatrolPoint.position);
                    }
                }
            }

        }
    }
}
using Entity;
using UnityEngine;
using Utilities;

namespace AI
{
    public class SimplePatrol : PatrolBase
    {
        [Header("Components")]
        [SerializeField] private MovementController movement;

        // Very simple patrol point
        // between two points, dest point must be to the right.
        // patrols from start point to dest point to right.
        
        [SerializeField] private Transform patrolPoint0;
        [SerializeField] private Transform patrolPoint1;

        private int _currentTargetPatrolPointIndex;
        private FacingDirection _facingDirection = FacingDirection.Right;

        private const float Speed = 2f;

        private void Start()
        {
            Debug.Assert(patrolPoint0 != null, this);
            Debug.Assert(patrolPoint1 != null, this);
            _currentTargetPatrolPointIndex = 0;
        }

        public override void UpdatePatrol()
        {
            MoveTowardsDestination();
        }

        private void MoveTowardsDestination()
        {
            Transform targetPatrolPoint = UpdateTargetPatrolPoint();
            
            UpdateFacingDirection(targetPatrolPoint);
            
            MoveInDirection();
        }

        private Transform UpdateTargetPatrolPoint()
        {
            Transform targetPatrolPoint = _currentTargetPatrolPointIndex == 0 ? patrolPoint0 : patrolPoint1;

            // If we're close enough to our destination point, update the destination
            const float distanceThreshold = 0.1f;
            if (Vector2.Distance(transform.position, targetPatrolPoint.position) < distanceThreshold)
            {
                _currentTargetPatrolPointIndex = (_currentTargetPatrolPointIndex + 1) % 2;
                targetPatrolPoint = _currentTargetPatrolPointIndex == 0 ? patrolPoint0 : patrolPoint1;
            }

            return targetPatrolPoint;
        }

        private void UpdateFacingDirection(Transform currentTargetPatrolPoint)
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
        }

        private void MoveInDirection()
        {
            // Move
            float fallSpeed = !movement.customCollider2D.GetCollisionBelow() ? Physics2D.gravity.y : 0f;
            movement.MoveAtSpeed(new Vector2((float)_facingDirection * Speed, fallSpeed));
        }

        private void OnDrawGizmosSelected()
        {
            if (!patrolPoint0 || !patrolPoint1) return;
            
            var patrolPoint0Position = patrolPoint0.position;
            var patrolPoint1Position = patrolPoint1.position;
                
            const float wireSphereRadius = 0.5f;
            Gizmos.DrawWireSphere(patrolPoint0Position, wireSphereRadius);
            Gizmos.DrawWireSphere(patrolPoint1Position, wireSphereRadius);
                
            Gizmos.DrawLine(patrolPoint0Position, patrolPoint1Position);
        }
    }
}
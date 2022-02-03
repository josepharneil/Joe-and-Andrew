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
            Transform targetPatrolPoint = patrolPath.UpdatePatrolPath(transform, distanceThreshold, ignorePatrolPointY);
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
            patrolPath.DrawGizmos();
        }
    }
}
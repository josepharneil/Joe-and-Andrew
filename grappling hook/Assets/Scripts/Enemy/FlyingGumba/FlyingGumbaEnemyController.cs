using AI;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class FlyingGumbaEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private FacingDirection _facingDirection = FacingDirection.Right;
        
        [Header("Movement")]
        [SerializeField] private float patrolPointDistanceThreshold = 1f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool movesFasterFurtherAway = true;

        private void OnValidate()
        {
            patrolPath.Validate();
        }

        private void OnDrawGizmosSelected()
        {
            patrolPath.DrawGizmos();
        }
        
        [UsedImplicitly] public void UpdatePatrol()
        {
            FlyTowardsTarget();
            spriteRenderer.flipX = _facingDirection != FacingDirection.Right;
        }

        private void FlyTowardsTarget()
        {
            // Current target patrol point.
            Transform targetPatrolPoint = patrolPath.UpdatePatrolPath(transform, patrolPointDistanceThreshold, false);

            Vector2 targetPosition = targetPatrolPoint.position;
            Vector2 thisPosition = transform.position;
            Vector2 direction = (targetPosition - thisPosition);
            Vector2 moveVector = direction * speed;
            if (!movesFasterFurtherAway)
            {
                moveVector.Normalize();
            }

            movementController.Move(moveVector);
            
            if (moveVector.x > 0)
            {
                _facingDirection = FacingDirection.Left;
            }
            else if (moveVector.x > 0)
            {
                _facingDirection = FacingDirection.Right;
            }        
        }
    }
}
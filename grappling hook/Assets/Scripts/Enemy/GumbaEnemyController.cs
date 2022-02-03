using AI;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class GumbaEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private FacingDirection _facingDirection = FacingDirection.Right;
        
        [Header("Movement")]
        [SerializeField] private float patrolPointDistanceThreshold = 1f;
        [SerializeField] private float speed = 2f;
        
        [UsedImplicitly] public void UpdatePatrol()
        {
            WalkTowardsTarget();
            spriteRenderer.flipX = _facingDirection != FacingDirection.Right;
        }
        
        private void WalkTowardsTarget()
        {
            // Current target patrol point.
            Transform targetPatrolPoint = patrolPath.UpdatePatrolPath(transform, patrolPointDistanceThreshold, true);

            // Update facing direction
            if (transform.position.IsRightOf(targetPatrolPoint.position))
            {
                _facingDirection = FacingDirection.Left;
            }
            else if (transform.position.IsLeftOf(targetPatrolPoint.position))
            {
                _facingDirection = FacingDirection.Right;
            }
            
            // Movement
            float fallSpeed = !movementController.customCollider2D.GetCollisionBelow() ? Physics2D.gravity.y : 0f;
            Vector2 moveVector = new Vector2((float)_facingDirection * speed, fallSpeed);
            movementController.Move(moveVector);
        }
    }
}
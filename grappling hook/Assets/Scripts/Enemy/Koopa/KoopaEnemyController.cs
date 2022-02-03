using AI;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class KoopaEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController movementController;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private FacingDirection _facingDirection = FacingDirection.Right;
        
        [Header("Movement")]
        [SerializeField] private float patrolPointDistanceThreshold = 1f;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 5f;

        [Header("CanSeeTarget")] 
        [SerializeField] private Transform chaseTarget;
        [SerializeField] private float sightRange = 10f;
        
        #region UnityEvents
        private void OnDrawGizmosSelected()
        {
            patrolPath.DrawGizmos();
            
            if (chaseTarget == null) return;
            var thisPosition = transform.position;
            var targetPosition = chaseTarget.position;
            Gizmos.DrawRay(thisPosition, (targetPosition - thisPosition).normalized * sightRange);
            Gizmos.DrawWireSphere(thisPosition, sightRange);
        }
        
        private void OnValidate()
        {
            patrolPath.Validate();
        }
        #endregion UnityEvents
        
        
        #region Patrol
        [UsedImplicitly] public void UpdatePatrol()
        {
            // Current target patrol point.
            Transform targetPatrolPoint = patrolPath.UpdatePatrolPath(transform, patrolPointDistanceThreshold, true);
            MoveTowardsTarget(targetPatrolPoint, patrolSpeed);
        }

        private void MoveTowardsTarget(Transform moveTarget, float moveSpeed)
        {
            // Update facing direction
            if (transform.position.IsRightOf(moveTarget.position))
            {
                _facingDirection = FacingDirection.Left;
            }
            else if (transform.position.IsLeftOf(moveTarget.position))
            {
                _facingDirection = FacingDirection.Right;
            }
            
            // Movement
            float fallSpeed = !movementController.customCollider2D.GetCollisionBelow() ? Physics2D.gravity.y : 0f;
            Vector2 moveVector = new Vector2((float)_facingDirection * moveSpeed, fallSpeed);
            movementController.Move(moveVector);
            
            spriteRenderer.flipX = _facingDirection != FacingDirection.Right;
        }
        #endregion Patrol

        #region CanSeeTarget
        [UsedImplicitly] public bool CanSeeTarget()
        {
            Vector2 thisPosition = (Vector2)transform.position;
            Vector2 targetPosition = chaseTarget.position;

            return (targetPosition - thisPosition).sqrMagnitude < sightRange * sightRange;
        }
        #endregion CanSeeTarget

        #region ChaseTarget
        [UsedImplicitly] public void UpdateChase()
        {
            MoveTowardsTarget(chaseTarget, chaseSpeed);
        }
        #endregion ChaseTarget
    }
}
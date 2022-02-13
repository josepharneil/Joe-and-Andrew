using AI;
using Entity;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Enemy
{
    public class FlyingDiverEnemyController : MonoBehaviour, IPatroller, IChaser, IAttacker
    {
        [Header("Components")]
        [SerializeField] private MovementController _movementController;
        [SerializeField] private PatrolPath _patrolPath;
        
        [Header("Movement")]
        [SerializeField] private float _patrolPointDistanceThreshold = 1f;
        [SerializeField] private float _patrolSpeed = 2f;
        [SerializeField] private float _chaseSpeed = 5f;
        [SerializeField] private float _chaseHeight = 4f;

        [Header("Chasing")] 
        [SerializeField] private Transform _chaseTarget;
        [SerializeField] private SightRaycast _sight;

        [Header("Attacking")]
        [SerializeField] private float _attackDistanceThreshold = 10;
        
        [Header("Dive Bombing")]
        [SerializeField] private float _diveBombCooldownDuration = 2f;
        [SerializeField] private float _diveBombDistance = 10f;
        [SerializeField] private float _diveBombSpeed = 20f;
        [SerializeField] private float _diveBombHeight = 4f;
        [HideInInspector] public bool DiveBombIsOnCooldown = true; // Used in Bolt
        private float _diveBombCooldownTimer = 0f;
        private Vector2 _attackPosition = Vector2.zero;
        private Vector2 _diveBombDestination;
        private bool _hasReachedAttackHeight = false;

        #region UnityEvents
        private void OnDrawGizmosSelected()
        {
            _patrolPath.DrawGizmos();
            // _sight.DrawGizmos();
            DrawAttackGizmos();
        }

        private void Start()
        {
            _sight.Setup(transform, _chaseTarget);
            Debug.Assert(_sight.GetSightRange() > _attackDistanceThreshold,
                "Sight range should be greater than the attack range.", this);
        }

        private void OnValidate()
        {
            _patrolPath.Validate();
            _sight.Setup(transform, _chaseTarget);
        }

        #endregion

        private void MoveTowardsTarget(Vector2 moveTarget, float moveSpeed)
        {
            Vector2 thisPosition = transform.position;
            Vector2 direction = thisPosition.DirectionTo(moveTarget).normalized;
            Vector2 moveVector = direction * moveSpeed;
            _movementController.Move(moveVector);
        }
        
        public void UpdatePatrol()
        {
            // Current target patrol point.
            Transform targetPatrolPoint = _patrolPath.UpdatePatrolPath(transform, _patrolPointDistanceThreshold, false);
            Vector2 position = (Vector2)targetPatrolPoint.position;
            MoveTowardsTarget(position, _patrolSpeed);
        }

        #region Chasing
        
        public void UpdateChase()
        {
            Vector2 chasePos = (Vector2)_chaseTarget.position + (Vector2.up * _chaseHeight);
            MoveTowardsTarget(chasePos, _chaseSpeed);
        }

        public bool CanDetectTarget()
        {
            return _sight.CanSeeTarget();
        }
        #endregion
        
        #region Attack
        
        public bool IsInAttackRange()
        {
            // Simple range check.
            return transform.position.DistanceToSquared(_chaseTarget.position) <
                   (_attackDistanceThreshold * _attackDistanceThreshold);
        }
        
        [UsedImplicitly] public void OnEnterAttack()
        {
            _diveBombCooldownTimer = 0f;
            DiveBombIsOnCooldown = true;
            _attackPosition = transform.position;
        }

        public void UpdateAttack()
        {
            if (DiveBombIsOnCooldown)
            {
                UpdateDiveBombCooldown();
            }
            // not on cooldown
            else
            {
                UpdateDiveBomb();
            }
        }

        private void UpdateDiveBombCooldownMovement()
        {
            if (transform.position.y < (_chaseTarget.position.y + _diveBombHeight) && (!_hasReachedAttackHeight))
            {
                Vector2 diveBombPosition = (Vector2)_attackPosition + (Vector2.up * _diveBombHeight);
                MoveTowardsTarget(diveBombPosition, _chaseSpeed);
            }
            else if (transform.position.y > (_chaseTarget.position.y + _diveBombHeight))
            {
                _hasReachedAttackHeight = true;
            }
        }

        private void UpdateDiveBombCooldown()
        {
            UpdateDiveBombCooldownMovement();
            _diveBombCooldownTimer += Time.deltaTime;
            if (_diveBombCooldownTimer > _diveBombCooldownDuration)
            {
                _diveBombCooldownTimer = 0f;
                DiveBombIsOnCooldown = false;
                
                Vector2 thisPosition = transform.position;
                Vector2 chasePosition = _chaseTarget.position;
                RaycastHit2D hit = Physics2D.Raycast(thisPosition, 
                    thisPosition.DirectionToNormalized(chasePosition), 
                    _diveBombDistance, _movementController.customCollider2D.GetCollisionMask().value);
                if (hit)
                {
                    _diveBombDestination = hit.point;
                }
                else
                {
                    _diveBombDestination = thisPosition +
                                           (thisPosition.DirectionToNormalized(chasePosition) * _diveBombDistance);
                }
            }
        }

        private void UpdateDiveBomb()
        {
            MoveTowardsTarget(_diveBombDestination, _diveBombSpeed);
            // If we're close to reaching the target, go on cooldown
            if (transform.position.DistanceToSquared(_diveBombDestination) < 0.5f)
            {
                _hasReachedAttackHeight = false;
                DiveBombIsOnCooldown = true;
                _attackPosition = transform.position;
            }
        }
        
        private void DrawAttackGizmos()
        {
            if (DiveBombIsOnCooldown)
            {
                Vector2 thisPosition = transform.position;
                Vector2 chasePosition = _chaseTarget.position;
                Vector2 normDirToChase = thisPosition.DirectionToNormalized(chasePosition);

                RaycastHit2D hit = Physics2D.Raycast(thisPosition, 
                    normDirToChase,
                    _diveBombDistance, _movementController.customCollider2D.GetCollisionMask().value);
                Vector2 rayDirection;
                if (hit)
                {
                    rayDirection = normDirToChase * hit.distance;
                }
                else
                {
                    rayDirection = normDirToChase * _diveBombDistance;
                }
                
                Handles.Label(thisPosition + (Vector2.up * 2f), (_diveBombCooldownDuration - _diveBombCooldownTimer).ToString("n2"));
                Gizmos.DrawRay(thisPosition, rayDirection);
            }
            else
            {
                Gizmos.DrawLine(transform.position, _diveBombDestination);
            }
        }
        #endregion
    }
}
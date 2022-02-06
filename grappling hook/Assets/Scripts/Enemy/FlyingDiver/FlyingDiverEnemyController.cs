using System;
using AI;
using Entity;
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

        [Header("Chasing")] 
        [SerializeField] private Transform _chaseTarget;
        [SerializeField] private SightRaycast _sight;

        [Header("Attacking")]
        [SerializeField] private float _attackDistanceThreshold = 10;
        

        #region UnityEvents
        private void OnDrawGizmosSelected()
        {
            _patrolPath.DrawGizmos();
            _sight.DrawGizmos();
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
        
        private void MoveTowardsTarget(Transform moveTarget, float moveSpeed)
        {
            Vector2 targetPosition = moveTarget.position;
            Vector2 thisPosition = transform.position;
            Vector2 direction = thisPosition.DirectionTo(targetPosition);
            Vector2 moveVector = (direction * moveSpeed).normalized;
            _movementController.Move(moveVector);
        }
        
        public void UpdatePatrol()
        {
            // Current target patrol point.
            Transform targetPatrolPoint = _patrolPath.UpdatePatrolPath(transform, _patrolPointDistanceThreshold, false);
            MoveTowardsTarget(targetPatrolPoint, _patrolSpeed);
        }

        public void UpdateChase()
        {
            MoveTowardsTarget(_chaseTarget, _chaseSpeed);
        }

        public bool CanDetectTarget()
        {
            return _sight.CanSeeTarget();
        }
        
        public void UpdateAttack()
        {
            // Dive bomb!
        }

        public bool IsInAttackRange()
        {
            // Simple range check.
            return transform.position.DistanceToSquared(_chaseTarget.position) <
                   (_attackDistanceThreshold * _attackDistanceThreshold);
        }
    }
}
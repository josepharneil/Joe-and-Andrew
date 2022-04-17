using System;
using System.Collections.Generic;
using Entity;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Enemy
{
    public class DemoBossEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController _movementController;
        [SerializeField] private BoxCollider2D _arenaBox;
        [SerializeField] private GameObject _target;

        // [] charging
        private Vector2 _chargeDestination;
        [SerializeField] private float _chargeWindUpDuration = 1f;
        [SerializeField] private float _chargeSpeed = 10f;
        [SerializeField] private float _chargeDistance = 5f;
        [SerializeField] private float _chargeRecoveryDuration = 0.5f;

        private void Update()
        {
            _movementController.Move(Vector2.down * 9.81f);
        }

        public float GetChargeWindUpDuration()
        {
            return _chargeWindUpDuration;
        }

        public float GetChargeRecoveryDuration()
        {
            return _chargeRecoveryDuration;
        }

        public bool TargetIsInBossArea()
        {
            return _arenaBox.bounds.Contains(_target.transform.position);
        }

        public void SetChargeDestination()
        {
            Vector2 thisPosition = transform.position;
            Vector2 directionNormToTarget = thisPosition.DirectionToNormalized(_target.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(thisPosition, directionNormToTarget, _chargeDistance, _movementController.customCollider2D.GetCollisionMask().value);
            if (hit)
            {
                _chargeDestination = hit.point - (directionNormToTarget * 3f);
            }
            else
            {
                _chargeDestination = thisPosition + (directionNormToTarget * _chargeDistance);
            }
        }

        public bool IsChargeAttackDone()
        {
            return transform.position.DistanceToSquared(_chargeDestination) < Mathf.Pow(1f, 2);
        }

        public void ChargeAttack()
        {
            _movementController.Move(transform.position.DirectionToNormalized(_chargeDestination) * _chargeSpeed);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, _chargeDestination);
            Gizmos.DrawWireSphere(_chargeDestination, 0.2f);
        }
#endif
    }
}
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

        [Header("Charging")]
        [SerializeField] private float _chargeWindUpDuration = 1f;
        [SerializeField] private float _chargeSpeed = 10f;
        // [SerializeField] private float _chargeDistance = 5f;
        [SerializeField] private float _chargeRecoveryDuration = 0.5f;
        [SerializeField] private Transform _chargeDestinationLeft;
        [SerializeField] private Transform _chargeDestinationRight;
        [SerializeField] private bool _chargingLeft = false;
        private Vector2 _chargeDestination;
        

        private void Update()
        {
            // _movementController.Move(Vector2.down * 9.81f);
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
            // Vector2 thisPosition = transform.position;
            _chargeDestination = (_chargingLeft ? _chargeDestinationLeft : _chargeDestinationRight).position;


            // Vector2 directionNormToTarget = thisPosition.DirectionToNormalized(_target.transform.position);
            // RaycastHit2D hit = Physics2D.Raycast(thisPosition, directionNormToTarget, _chargeDistance, _movementController.customCollider2D.GetCollisionMask().value);
            // if (hit)
            // {
            // _chargeDestination = hit.point - (directionNormToTarget * 3f);
            // }
            // else
            // {
            // _chargeDestination = thisPosition + (directionNormToTarget * _chargeDistance);
            // }
        }

        public bool IsChargeAttackDone()
        {
            // Vector2 onlyXThisPosition = new Vector2(transform.position.x, 0f);
            // Vector2 onlyXDestination = new Vector2(_chargeDestination.x, 0f);
            // const float distanceThreshold = 2f;
            // return Mathf.Pow(transform.position.x - _chargeDestination.x, 2) < Mathf.Pow(distanceThreshold, 2);

            // return onlyXThisPosition.DistanceToSquared(onlyXDestination) < Mathf.Pow(distanceThreshold, 2);

            float sqDistBetweenThisAndDestination = Mathf.Pow(transform.position.x - _chargeDestination.x, 2);
            const float tolerance = 0.25f;
            float sqHalfScalePlusTolerance = Mathf.Pow((transform.localScale.x / 2f) + tolerance, 2);
            if (sqDistBetweenThisAndDestination < sqHalfScalePlusTolerance)
            {
                _chargingLeft = !_chargingLeft;
                return true;
            }

            return false;
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
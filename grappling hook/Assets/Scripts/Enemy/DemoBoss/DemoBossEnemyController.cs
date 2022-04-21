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
        [SerializeField] private CameraShakeData _cameraShakeData;
        private bool _chargingLeft = false;
        private int _chargeCounter = 0;
        [SerializeField] private int _chargeMaxNumber = 4;
        private Vector2 _chargeDestination;
        

        private void Update()
        {
            // _movementController.Move(Vector2.down * 9.81f);
        }

        #region ActionSelection
        
        #endregion


        public bool TargetIsInBossArea()
        {
            return _arenaBox.bounds.Contains(_target.transform.position);
        }

        #region ChargeAttack

        public void InitialiseChargeBarrageAttack()
        {
            _chargeCounter = 0;
            _chargingLeft = transform.position.IsRightOf(_target.transform.position);
        }

        public float GetChargeWindUpDuration()
        {
            return _chargeWindUpDuration;
        }

        public float GetChargeRecoveryDuration()
        {
            return _chargeRecoveryDuration;
        }

        public void SetChargeDestination()
        {
            _chargeDestination = (_chargingLeft ? _chargeDestinationLeft : _chargeDestinationRight).position;
        }

        public bool HasChargeAttackReachedDestination()
        {
            float sqDistBetweenThisAndDestination = Mathf.Abs(transform.position.x - _chargeDestination.x);
            const float tolerance = 0f;//0.25f;
            float sqHalfScalePlusTolerance = Mathf.Abs((transform.localScale.x / 2f) + tolerance);

            bool destinationReached = sqDistBetweenThisAndDestination < sqHalfScalePlusTolerance;
            if (destinationReached)
            {
                // Switch direction here for now...
                _chargingLeft = !_chargingLeft;
                CameraManager.Instance.Shake.ShakeCamera(_cameraShakeData);
                _chargeCounter++;
            }

            return destinationReached;
        }

        public bool IsChargeAttackDone()
        {
            return _chargeCounter >= _chargeMaxNumber;
        }
        
        public void ChargeAttack()
        {
            _movementController.Move(transform.position.DirectionToNormalized(_chargeDestination) * _chargeSpeed);
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, _chargeDestination);
            Gizmos.DrawWireSphere(_chargeDestination, 0.2f);
        }
#endif
    }
}
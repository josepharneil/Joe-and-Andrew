using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Enemy
{
    public class DemoBossEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController _movementController;
        [SerializeField] private BoxCollider2D _arenaBox;
        [SerializeField] private GameObject _target;
        [SerializeField] private Unity.VisualScripting.StateMachine _stateMachine;

        [Header("Charge Attack")]
        [SerializeField] private float _chargeWindUpDuration = 1f;
        [SerializeField] private float _chargeSpeed = 10f;
        // [SerializeField] private float _chargeDistance = 5f;
        [SerializeField] private float _chargeRecoveryDuration = 0.5f;
        [SerializeField] private int _chargeMaxNumber = 4;
        [SerializeField] private Transform _chargeDestinationLeft;
        [SerializeField] private Transform _chargeDestinationRight;
        [SerializeField] private CameraShakeData _cameraShakeData;
        private bool _chargingLeft = false;
        private int _chargeCounter = 0;
        private Vector2 _chargeDestination;

        [Header("Projectile Attack")]
        [SerializeField] private GameObject _projectile;
        [SerializeField] private float _projectileDuration = 4f;
        [SerializeField] private float _projectileKnockback = 2f;
        [SerializeField] private float _projectileMoveSpeed = 6f;
        [SerializeField] private int _projectileDamage = 5;

        #region ActionSelection
        
        #endregion

        #region Idle

        public bool TargetIsInBossArea()
        {
            return _arenaBox.bounds.Contains(_target.transform.position);
        }
        
        #endregion


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

        #region ProjectileAttack

        private readonly List<DemoBossProjectile> _liveProjectiles = new();
        private int _projectileCounter = 0;
        private bool _fireProjectilesToRight = true;
        [SerializeField] private int _maxNumberOfProjectiles = 10;

        public void InitialiseProjectileBarrage()
        {
            _projectileCounter = 0;
            _fireProjectilesToRight = _target.transform.position.IsRightOf(transform.position);
        }
        
        public void ShootProjectile()
        {
            Transform thisTransform = transform;
            Vector3 thisPosition = thisTransform.position;
            GameObject projectileGameObject = Instantiate(_projectile, thisPosition, Quaternion.identity);
            DemoBossProjectile projectile = projectileGameObject.GetComponent<DemoBossProjectile>();
            Debug.Assert(projectile, "Should be a fireball component", this);
            if (!projectile) return;

            Vector2 direction = _fireProjectilesToRight ? Vector2.right : Vector2.left;
                
            projectile.Initialise(_target.gameObject.layer, direction);
            _liveProjectiles.Add(projectile);

            _projectileCounter++;
        }

        public void UpdateProjectiles()
        {
            if(_liveProjectiles.Count == 0) return;
            
            List<int> indexesToDelete = new List<int>
            {
                Capacity = _liveProjectiles.Count
            };
            for (int index = 0; index < _liveProjectiles.Count; index++)
            {
                DemoBossProjectile projectile = _liveProjectiles[index];
                projectile.LifespanTimer += Time.deltaTime;
                if (projectile.LifespanTimer > _projectileDuration)
                {
                    indexesToDelete.Add(index);
                }
                else if (projectile.HasHitTarget)
                {
                    indexesToDelete.Add(index);

                    _target.TryGetComponent(out EntityHitbox entityHitbox);
                    if (entityHitbox)
                    {
                        entityHitbox.Hit(new EntityHitData 
                        {
                            DealsDamage = true,
                            DamageToHealth = _projectileDamage,

                            DealsKnockback = true,
                            KnockbackStrength = _projectileKnockback,
                            KnockbackOrigin = projectile.gameObject.transform.position
                        });
                    }
                }
                // Hit by player for example
                // else if (projectile.Destroyed)
                // {
                    // indexesToDelete.Add(index);
                // }

                projectile.UpdatePath(_projectileMoveSpeed);
            }

            foreach (int indexToDelete in indexesToDelete)
            {
                GameObject gameObjectToDestroy = _liveProjectiles[indexToDelete].gameObject;
                _liveProjectiles.RemoveAt(indexToDelete);
                Destroy(gameObjectToDestroy);
            }
        }

        public bool IsProjectileBarrageActionDone()
        {
            return _projectileCounter >= _maxNumberOfProjectiles;
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
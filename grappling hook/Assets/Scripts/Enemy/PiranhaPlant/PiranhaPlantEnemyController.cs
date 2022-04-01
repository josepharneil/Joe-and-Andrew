using System.Collections.Generic;
using AI;
using Entity;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Enemy
{
    public class PiranhaPlantEnemyController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        
        [Header("Attack")]
        [SerializeField] private float attacksPerSecond = 0.25f;
        [SerializeField] private GameObject piranhaProjectile;
        [SerializeField] private float fireballDuration = 4f;
        [SerializeField] private int fireballDamage = 5;
        [SerializeField] private float fireballMoveSpeed = 6f;
        [SerializeField] private float knockbackStrength = 5f;
        private float _attackTimer = 0f;
        private bool _attackIsOnCooldown = false;
        
        [SerializeField] private SightRaycast _sight;
        
        // TODO
        // Note to self:
        // A possible eventual implementation of projectiles could involve a "ProjectileManager"
        // that stores a bunch of lists of data (ie a struct of lists instead of a list of structs)
        // That way we can very quickly iterate over ALL projectiles
        // This could get quite complex, but could be important if we do any bullet hell style levels for efficient
        // bullet hells.
        private readonly List<PiranhaFireball> _livePiranhaProjectiles = new List<PiranhaFireball>();

        #region UnityEvents

        private void OnValidate()
        {
            _sight.Setup(transform, target);
        }

        private void Start()
        {
            _sight.Setup(transform, target);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            _sight.DrawGizmos();
            if (_attackIsOnCooldown)
            {
                Handles.Label(transform.position + Vector3.up, ((1f / attacksPerSecond) - _attackTimer).ToString("n2"));
            }
        }
#endif

        #endregion

        #region CanSeeTarget
        [UsedImplicitly] public bool CanSeeTarget()
        {
            return _sight.CanSeeTarget();
        }
        #endregion CanSeeTarget

        #region AttackTarget
        /// <summary>
        /// Attempt to shoot a new projectile.
        /// </summary>
        [UsedImplicitly] public void TryShootProjectile()
        {
            if(!_attackIsOnCooldown)
            {
                ShootProjectile();
                _attackIsOnCooldown = true;
            }
        }

        /// <summary>
        /// Update attack timer.
        /// </summary>
        [UsedImplicitly] public void UpdateAttackTimer()
        {
            if (_attackIsOnCooldown)
            {
                _attackTimer += Time.deltaTime;
                if (_attackTimer > 1f / attacksPerSecond)
                {
                    _attackIsOnCooldown = false;
                    _attackTimer = 0f;
                }
            }
        }
        
        /// <summary>
        /// Updates the state of all projectiles.
        /// </summary>
        [UsedImplicitly] public void UpdateProjectiles()
        {
            if(_livePiranhaProjectiles.Count == 0) return;
            
            List<int> indexesToDelete = new List<int>
            {
                Capacity = _livePiranhaProjectiles.Count
            };
            for (int index = 0; index < _livePiranhaProjectiles.Count; index++)
            {
                PiranhaFireball projectile = _livePiranhaProjectiles[index];
                projectile.LifespanTimer += Time.deltaTime;
                if (projectile.LifespanTimer > fireballDuration)
                {
                    indexesToDelete.Add(index);
                }
                else if (projectile.HasHitTarget)
                {
                    indexesToDelete.Add(index);

                    target.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
                    if (entityHitbox)
                    {
                        entityHitbox.Hit(new EntityHitData()
                        {
                            DealsDamage = true,
                            DamageToHealth = fireballDamage,

                            DealsKnockback = true,
                            KnockbackStrength = knockbackStrength,
                            KnockbackOrigin = projectile.gameObject.transform.position
                        });
                    }
                }
                // Hit by player for example
                else if (projectile.Destroyed)
                {
                    indexesToDelete.Add(index);
                }

                projectile.UpdatePath(fireballMoveSpeed);
            }

            foreach (int indexToDelete in indexesToDelete)
            {
                GameObject gameObjectToDestroy = _livePiranhaProjectiles[indexToDelete].gameObject;
                _livePiranhaProjectiles.RemoveAt(indexToDelete);
                Destroy(gameObjectToDestroy);
            }
        }

        /// <summary>
        /// Checks if all projectiles have been destroyed.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly] public bool AreAllProjectilesDestroyed()
        {
            return _livePiranhaProjectiles.Count == 0;
        }
        
        private void ShootProjectile()
        {
            var thisTransform = transform;
            var thisPosition = thisTransform.position;
            GameObject projectile = Instantiate(piranhaProjectile, thisPosition, Quaternion.identity, thisTransform);
            PiranhaFireball piranhaFireball = projectile.GetComponent<PiranhaFireball>();
            Debug.Assert(piranhaFireball, "Should be a fireball component", this);
            if (!piranhaFireball) return;
            
            piranhaFireball.Initialise(target.gameObject.layer, thisPosition.DirectionToNormalized(target.position));
            _livePiranhaProjectiles.Add(piranhaFireball);
        }
        #endregion AttackTarget
    }
}
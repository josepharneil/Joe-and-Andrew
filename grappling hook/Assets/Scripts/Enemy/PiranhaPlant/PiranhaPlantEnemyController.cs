using System;
using System.Collections.Generic;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class PiranhaPlantEnemyController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float sightRange;
        
        [Header("Attack")]
        [SerializeField] private float attacksPerSecond = 0.25f;
        [SerializeField] private GameObject piranhaProjectile;
        [SerializeField] private float fireballDuration = 4f;
        [SerializeField] private int fireballDamage = 5;
        [SerializeField] private float fireballMoveSpeed = 6f;
        [SerializeField] private float knockbackStrength = 5f;
        private float _attackTimer = 0f;
        private bool _attackIsOnCooldown = false;
        
        // TODO
        // Note to self:
        // A possible eventual implementation of projectiles could involve a "ProjectileManager"
        // that stores a bunch of lists of data (ie a struct of lists instead of a list of structs)
        // That way we can very quickly iterate over ALL projectiles
        // This could get quite complex, but could be important if we do any bullet hell style levels for efficient
        // bullet hells.
        private readonly List<PiranhaFireball> _livePiranhaProjectiles = new List<PiranhaFireball>();

        #region UnityEvents

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;
            var thisPosition = transform.position;
            var targetPosition = target.position;
            Gizmos.DrawRay(thisPosition, (targetPosition - thisPosition).normalized * sightRange);
            Gizmos.DrawWireSphere(thisPosition, sightRange);
        }

        #endregion

        #region CanSeeTarget
        [UsedImplicitly] public bool CanSeeTarget()
        {
            Vector2 thisPosition = (Vector2)transform.position;
            Vector2 targetPosition = target.position;

            return (targetPosition - thisPosition).sqrMagnitude < sightRange * sightRange;
        }
        #endregion CanSeeTarget

        #region AttackTarget
        /// <summary>
        /// Attempt to shoot a new projectile.
        /// </summary>
        [UsedImplicitly] public void TryShootProjectile()
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
            else
            {
                ShootProjectile();
                _attackIsOnCooldown = true;
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
                projectile.lifespanTimer += Time.deltaTime;
                if (projectile.lifespanTimer > fireballDuration)
                {
                    indexesToDelete.Add(index);
                }

                if (projectile.hasHitPlayer)
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

                projectile.transform.Translate(projectile.direction.normalized * fireballMoveSpeed * Time.deltaTime);
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
            var thisPosition = transform.position;
            GameObject projectile = Instantiate(piranhaProjectile, thisPosition, Quaternion.identity);
            PiranhaFireball piranhaFireball = projectile.GetComponent<PiranhaFireball>();
            piranhaFireball.targetLayerMask = target.gameObject.layer;
            piranhaFireball.direction = target.position - thisPosition;
            _livePiranhaProjectiles.Add(piranhaFireball);
        }
        #endregion AttackTarget
    }
}
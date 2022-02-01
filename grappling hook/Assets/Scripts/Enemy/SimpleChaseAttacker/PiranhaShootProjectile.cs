using System.Collections.Generic;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class PiranhaShootProjectile : MonoBehaviour
    {
        [SerializeField] private float attacksPerSecond = 1;
        [SerializeField] private GameObject piranhaProjectile;
        [SerializeField] private Transform target;
        [SerializeField] private float fireballDuration = 4f;
        [SerializeField] private int fireballDamage = 5;
        [SerializeField] private float fireballMoveSpeed = 2f;
        [SerializeField] private float knockbackStrength = 5f;
        private float _attackTimer = 0f;
        private bool _attackIsOnCooldown = false;

        private readonly List<PiranhaFireball> _livePiranhaProjectiles = new List<PiranhaFireball>();

        // [UsedImplicitly] public void UpdateAttack()
        // {
        //     TryShootProjectile();
        //     UpdateProjectiles();
        // }

        /// <summary>
        /// Attempt to shoot a new projectile.
        /// </summary>
        [UsedImplicitly] public void TryShootProjectile()
        {
            if (_attackIsOnCooldown)
            {
                _attackTimer += Time.deltaTime;
                if (_attackTimer > (float)1 / attacksPerSecond)
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
    }
}
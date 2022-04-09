using System;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// If other entities touch this entity, damage them.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityDamageOnTouch : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        
        [Header("Components")]
        [SerializeField] private LayerMask damagesWhat;

        [Header("Customisation")] 
        [SerializeField] private bool _instantDeath = false;
        [SerializeField] private int damage = 5;
        [SerializeField] private float knockbackStrength = 2f;
        [SerializeField] private EntityController entityController;

        private void Awake()
        {
            if (gameObject.GetComponent<EntityHealth>())
            {
                Debug.Assert(entityController, "Must have an entity controller to kill this enemy.", this);
            }
        }

        private void OnEnable()
        {
            if (gameObject.GetComponent<EntityHealth>())
            {
                entityController.OnEnemyDead += DisableEntityDamageOnTouch;
            }
        }

        private void OnDisable()
        {
            if (gameObject.GetComponent<EntityHealth>())
            {
                entityController.OnEnemyDead -= DisableEntityDamageOnTouch;
            }
        }

        private void DisableEntityDamageOnTouch()
        {
            isEnabled = false;
        }
        
        private void OnTriggerStay2D(Collider2D col)
        {
            if (!isEnabled)
            {
                return;
            }

            GameObject collidedGameObject = col.gameObject;
            if (((1 << collidedGameObject.layer) & damagesWhat) == 0) return;

            collidedGameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
            if (!entityHitbox) return;
            
            EntityHitData hitData = new EntityHitData
            {
                DealsDamage = true,
                DamageToHealth = damage,
                    
                DealsKnockback = true,
                KnockbackOrigin = transform.position,
                KnockbackStrength = knockbackStrength
            };

            if (_instantDeath)
            {
                entityHitbox.Kill(hitData);
            }
            else
            {
               entityHitbox.Hit(hitData);
            }
        }
    }
}


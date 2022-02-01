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
        [SerializeField] private int damage = 5;
        [SerializeField] private float knockbackStrength = 2f;

        // [SerializeField] private CustomCollider2D customCollider2D;

        [SerializeField] private EntityController entityController;

        private void Awake()
        {
            Debug.Assert(entityController, "Must have an entity controller to kill this enemy.", this);
        }

        private void OnEnable()
        {
            entityController.OnEnemyDead += DisableEntityDamageOnTouch;
        }

        private void OnDisable()
        {
            entityController.OnEnemyDead -= DisableEntityDamageOnTouch;
        }

        private void DisableEntityDamageOnTouch()
        {
            isEnabled = false;
        }

        // private void OnEnable()
        // {
        //     customCollider2D.OnTriggerEnter += OnCustomTriggerEnter;
        // }
        //
        // private void OnDisable()
        // {
        //     customCollider2D.OnTriggerEnter -= OnCustomTriggerEnter;
        // }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (!isEnabled)
            {
                return;
            }

            GameObject collidedGameObject = col.gameObject;
            if (((1 << collidedGameObject.layer) & damagesWhat) == 0) return;

            collidedGameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
            if (entityHitbox)
            {
                EntityHitData hitData = new EntityHitData
                {
                    DealsDamage = true,
                    DamageToHealth = damage,
                    
                    DealsKnockback = true,
                    KnockbackOrigin = transform.position,
                    KnockbackStrength = knockbackStrength
                };
                entityHitbox.Hit(hitData);
            }
        }
    }
}


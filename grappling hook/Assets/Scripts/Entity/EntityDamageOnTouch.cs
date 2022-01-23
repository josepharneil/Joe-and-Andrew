using System;
using Physics;
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

        [SerializeField] private CustomCollider2D customCollider2D;

        private void OnEnable()
        {
            customCollider2D.OnTriggerEnter += OnCustomTriggerEnter;
        }

        private void OnDisable()
        {
            customCollider2D.OnTriggerEnter -= OnCustomTriggerEnter;
        }

        private void OnCustomTriggerEnter(Collider2D col)
        {
            if (!isEnabled)
            {
                return;
            }

            GameObject collidedGameObject = col.gameObject;
            if ((collidedGameObject.gameObject.layer & damagesWhat) != 0) return;

            collidedGameObject.gameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
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

